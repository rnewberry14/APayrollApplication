using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace ClearPathPayroll.Tests;

public class UserDefinedFieldTests
{
    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        return results;
    }

    private static PayrollDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<PayrollDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new PayrollDbContext(options);
    }

    [Fact]
    public void DefinitionValidation_RejectsUnsafeFieldCode()
    {
        var definition = CreateDefinition();
        definition.FieldCode = "bonus<script>";

        var results = Validate(definition);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(UserDefinedFieldDefinition.FieldCode)));
    }

    [Fact]
    public void DefinitionValidation_RejectsScriptText()
    {
        var definition = CreateDefinition();
        definition.DefaultValue = "javascript:alert(1)";

        var results = Validate(definition);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(UserDefinedFieldDefinition.DefaultValue)));
    }

    [Fact]
    public void DefinitionValidation_ListFieldRequiresOptions()
    {
        var definition = CreateDefinition();
        definition.DataType = UserDefinedFieldDataType.List;
        definition.ListOptions = string.Empty;

        var results = Validate(definition);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(UserDefinedFieldDefinition.ListOptions)));
    }

    [Fact]
    public void ValueValidation_RejectsScriptText()
    {
        var value = new UserDefinedFieldValue
        {
            FieldDefinitionId = 1,
            EntityType = UserDefinedFieldAppliesTo.Employee,
            EntityId = 1,
            TextValue = "<script>alert(1)</script>"
        };

        var results = Validate(value);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(UserDefinedFieldValue.TextValue)));
    }

    [Fact]
    public async Task SaveDefinitionAsync_PersistsDefinitionLocally()
    {
        await using var context = CreateContext("user_defined_definition_save");
        context.Companies.Add(CreateCompany());
        await context.SaveChangesAsync();

        var service = new UserDefinedFieldService(context);
        await service.SaveDefinitionAsync(CreateDefinition());

        var saved = await context.UserDefinedFieldDefinitions.SingleAsync();
        Assert.Equal("BONUS_CODE", saved.FieldCode);
        Assert.Equal(UserDefinedFieldCalculationRole.InformationalOnly, saved.CalculationRole);
    }

    [Fact]
    public async Task SaveValueAsync_RejectsValueForWrongEntityType()
    {
        await using var context = CreateContext("user_defined_value_wrong_entity");
        context.Companies.Add(CreateCompany());
        var definition = CreateDefinition();
        context.UserDefinedFieldDefinitions.Add(definition);
        await context.SaveChangesAsync();

        var service = new UserDefinedFieldService(context);
        var value = new UserDefinedFieldValue
        {
            FieldDefinitionId = definition.UserDefinedFieldDefinitionId,
            EntityType = UserDefinedFieldAppliesTo.Company,
            EntityId = 1,
            TextValue = "Local value"
        };

        await Assert.ThrowsAsync<ValidationException>(() => service.SaveValueAsync(value));
    }

    [Fact]
    public async Task SaveValueAsync_ValidatesListOption()
    {
        await using var context = CreateContext("user_defined_value_list");
        context.Companies.Add(CreateCompany());
        var definition = CreateDefinition();
        definition.DataType = UserDefinedFieldDataType.List;
        definition.ListOptions = "A,B,C";
        context.UserDefinedFieldDefinitions.Add(definition);
        await context.SaveChangesAsync();

        var service = new UserDefinedFieldService(context);
        var value = new UserDefinedFieldValue
        {
            FieldDefinitionId = definition.UserDefinedFieldDefinitionId,
            EntityType = UserDefinedFieldAppliesTo.Employee,
            EntityId = 1,
            TextValue = "B"
        };

        await service.SaveValueAsync(value);

        Assert.Equal("B", (await context.UserDefinedFieldValues.SingleAsync()).TextValue);
    }

    private static Company CreateCompany()
    {
        return new Company
        {
            LegalName = "Demo Company LLC",
            FEIN = "00-0000000",
            PrimaryAddress = "100 Local Way",
            City = "Oklahoma City",
            State = "OK",
            ZipCode = "73102",
            IsActive = true
        };
    }

    private static UserDefinedFieldDefinition CreateDefinition()
    {
        return new UserDefinedFieldDefinition
        {
            CompanyId = 1,
            FieldName = "Bonus Code",
            FieldCode = "BONUS_CODE",
            AppliesTo = UserDefinedFieldAppliesTo.Employee,
            DataType = UserDefinedFieldDataType.Text,
            CalculationRole = UserDefinedFieldCalculationRole.InformationalOnly,
            IsActive = true
        };
    }
}
