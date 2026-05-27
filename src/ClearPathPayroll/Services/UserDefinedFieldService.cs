using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClearPathPayroll.Services;

public class UserDefinedFieldService
{
    private readonly PayrollDbContext _context;

    public UserDefinedFieldService(PayrollDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDefinedFieldDefinition>> GetDefinitionsByCompanyAsync(int companyId)
    {
        return await _context.UserDefinedFieldDefinitions
            .Where(definition => definition.CompanyId == companyId)
            .OrderBy(definition => definition.AppliesTo)
            .ThenBy(definition => definition.FieldName)
            .ToListAsync();
    }

    public async Task<List<UserDefinedFieldDefinition>> GetActiveDefinitionsForEntityAsync(
        int companyId,
        UserDefinedFieldAppliesTo appliesTo)
    {
        return await _context.UserDefinedFieldDefinitions
            .Where(definition => definition.CompanyId == companyId
                && definition.AppliesTo == appliesTo
                && definition.IsActive)
            .OrderBy(definition => definition.FieldName)
            .ToListAsync();
    }

    public async Task<UserDefinedFieldDefinition?> GetDefinitionByIdAsync(int definitionId)
    {
        return await _context.UserDefinedFieldDefinitions.FindAsync(definitionId);
    }

    public async Task<UserDefinedFieldDefinition> SaveDefinitionAsync(UserDefinedFieldDefinition definition)
    {
        Validate(definition);

        if (definition.UserDefinedFieldDefinitionId == 0)
        {
            definition.CreatedAt = DateTime.UtcNow;
            _context.UserDefinedFieldDefinitions.Add(definition);
        }
        else
        {
            definition.UpdatedAt = DateTime.UtcNow;
            _context.UserDefinedFieldDefinitions.Update(definition);
        }

        await _context.SaveChangesAsync();
        return definition;
    }

    public async Task<UserDefinedFieldDefinition> ActivateTemplateAsync(int companyId, PayrollFieldTemplate template)
    {
        var existing = await _context.UserDefinedFieldDefinitions
            .FirstOrDefaultAsync(definition => definition.CompanyId == companyId && definition.FieldCode == template.FieldCode);

        if (existing != null)
        {
            throw new ValidationException("This optional field template is already active for the selected company.");
        }

        var definition = new UserDefinedFieldDefinition
        {
            CompanyId = companyId,
            FieldName = template.FieldName,
            FieldCode = template.FieldCode,
            AppliesTo = template.AppliesTo,
            DataType = template.DataType,
            IncludeInPayrollCalculation = template.IncludeInPayrollCalculation,
            CalculationRole = template.CalculationRole,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        return await SaveDefinitionAsync(definition);
    }

    public async Task<List<UserDefinedFieldValue>> GetValuesForEntityAsync(
        UserDefinedFieldAppliesTo entityType,
        int entityId)
    {
        return await _context.UserDefinedFieldValues
            .Include(value => value.FieldDefinition)
            .Where(value => value.EntityType == entityType && value.EntityId == entityId)
            .OrderBy(value => value.FieldDefinition == null ? string.Empty : value.FieldDefinition.FieldName)
            .ToListAsync();
    }

    public async Task<UserDefinedFieldValue> SaveValueAsync(UserDefinedFieldValue value)
    {
        Validate(value);

        var definition = await _context.UserDefinedFieldDefinitions
            .FirstOrDefaultAsync(field => field.UserDefinedFieldDefinitionId == value.FieldDefinitionId);

        if (definition == null)
        {
            throw new ValidationException("Field definition was not found.");
        }

        if (definition.AppliesTo != value.EntityType)
        {
            throw new ValidationException("Field value entity type does not match the field definition.");
        }

        ValidateValueMatchesDataType(definition, value);

        if (value.UserDefinedFieldValueId == 0)
        {
            value.CreatedAt = DateTime.UtcNow;
            _context.UserDefinedFieldValues.Add(value);
        }
        else
        {
            value.UpdatedAt = DateTime.UtcNow;
            _context.UserDefinedFieldValues.Update(value);
        }

        await _context.SaveChangesAsync();
        return value;
    }

    private static void Validate(object model)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(model, context, results, true))
        {
            throw new ValidationException(results[0].ErrorMessage);
        }
    }

    private static void ValidateValueMatchesDataType(UserDefinedFieldDefinition definition, UserDefinedFieldValue value)
    {
        var hasValue = definition.DataType switch
        {
            UserDefinedFieldDataType.Text => !string.IsNullOrWhiteSpace(value.TextValue),
            UserDefinedFieldDataType.List => !string.IsNullOrWhiteSpace(value.TextValue),
            UserDefinedFieldDataType.Number => value.NumberValue.HasValue,
            UserDefinedFieldDataType.Currency => value.DecimalValue.HasValue,
            UserDefinedFieldDataType.Percent => value.DecimalValue.HasValue,
            UserDefinedFieldDataType.Date => value.DateValue.HasValue,
            UserDefinedFieldDataType.Boolean => value.BooleanValue.HasValue,
            _ => false
        };

        if (definition.IsRequired && !hasValue)
        {
            throw new ValidationException("A value is required for this custom field.");
        }

        if (definition.DataType == UserDefinedFieldDataType.List
            && !string.IsNullOrWhiteSpace(value.TextValue)
            && !ListContainsOption(definition.ListOptions, value.TextValue))
        {
            throw new ValidationException("List field value is not one of the configured options.");
        }
    }

    private static bool ListContainsOption(string? options, string value)
    {
        return (options ?? string.Empty)
            .Split(new[] { '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(option => string.Equals(option, value, StringComparison.OrdinalIgnoreCase));
    }
}
