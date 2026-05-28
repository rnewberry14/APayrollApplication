using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClearPathPayroll.Services;

/// <summary>
/// Service for managing companies.
/// </summary>
public class CompanyService
{
    private readonly PayrollDbContext _context;

    public CompanyService(PayrollDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets all active companies.
    /// </summary>
    public async Task<List<Company>> GetAllActiveCompaniesAsync()
    {
        return await _context.Companies
            .Where(c => c.IsActive)
            .OrderBy(c => c.LegalName)
            .ToListAsync();
    }

    public async Task<List<Company>> GetAllCompaniesAsync()
    {
        return await _context.Companies
            .OrderBy(c => c.LegalName)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a company by ID.
    /// </summary>
    public async Task<Company?> GetCompanyByIdAsync(int id)
    {
        return await _context.Companies.FindAsync(id);
    }

    public async Task<Company?> GetCompanyWithPayrollItemsAsync(int id)
    {
        return await _context.Companies
            .Include(c => c.PayrollItems.OrderBy(item => item.ItemCode))
            .FirstOrDefaultAsync(c => c.CompanyId == id);
    }

    /// <summary>
    /// Creates a new company.
    /// </summary>
    public async Task<Company> CreateCompanyAsync(Company company)
    {
        NormalizeEmployerFields(company);
        ValidateCompany(company);
        company.CreatedAt = DateTime.UtcNow;
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();
        return company;
    }

    /// <summary>
    /// Updates an existing company.
    /// </summary>
    public async Task<bool> UpdateCompanyAsync(Company company)
    {
        NormalizeEmployerFields(company);
        ValidateCompany(company);
        company.UpdatedAt = DateTime.UtcNow;
        _context.Companies.Update(company);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Company> SaveEmployerSetupAsync(Company company)
    {
        NormalizeEmployerFields(company);
        ValidateCompany(company);

        if (company.CompanyId == 0)
        {
            company.CreatedAt = DateTime.UtcNow;
            _context.Companies.Add(company);
        }
        else
        {
            company.UpdatedAt = DateTime.UtcNow;
            _context.Companies.Update(company);
        }

        await _context.SaveChangesAsync();
        return company;
    }

    public static string FormatFEIN(string? fein)
    {
        if (string.IsNullOrWhiteSpace(fein))
        {
            return string.Empty;
        }

        var trimmed = fein.Trim().Replace(" ", string.Empty);
        var digits = new string(trimmed.Where(char.IsDigit).ToArray());

        return digits.Length == 9
            ? $"{digits[..2]}-{digits[2..]}"
            : trimmed;
    }

    /// <summary>
    /// Deactivates a company (soft delete).
    /// </summary>
    public async Task<bool> DeactivateCompanyAsync(int id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null) return false;

        company.IsActive = false;
        company.UpdatedAt = DateTime.UtcNow;
        _context.Companies.Update(company);
        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Masks a FEIN for display purposes. Shows only first 2 digits and next 2 digits.
    /// Format: XX-XX-****
    /// Example: 12-3456789 -> 12-34-****
    /// SENSITIVE: FEIN is sensitive data and should never be logged or exposed unnecessarily.
    /// </summary>
    /// <param name="fein">The unmasked FEIN (9-10 digits)</param>
    /// <returns>Masked FEIN string in format XX-XX-****</returns>
    public static string MaskFEIN(string? fein)
    {
        if (string.IsNullOrEmpty(fein))
            return "XX-XX-****";

        var digits = fein.Replace("-", "").Trim();
        if (digits.Length < 5)
            return "XX-XX-****";

        // Show first 2 digits, next 2 digits, mask the rest
        return $"{digits.Substring(0, 2)}-{digits.Substring(2, 2)}-****";
    }

    public static string MaskAccountPlaceholder(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "Not set";
        }

        var trimmed = value.Trim();
        if (trimmed.Length <= 4)
        {
            return "****";
        }

        return $"****{trimmed[^4..]}";
    }

    private static void ValidateCompany(Company company)
    {
        var context = new ValidationContext(company);
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(company, context, results, true))
        {
            throw new ValidationException(results[0].ErrorMessage);
        }
    }

    private static void NormalizeEmployerFields(Company company)
    {
        company.FEIN = FormatFEIN(company.FEIN);
        company.SUIN = NormalizeIdentifier(company.SUIN);
        company.SEIN = NormalizeIdentifier(company.SEIN);
        company.SutaEmployerAccountNumberPlaceholder = NormalizeIdentifier(company.SutaEmployerAccountNumberPlaceholder);
        company.StateWithholdingAccountNumberPlaceholder = NormalizeIdentifier(company.StateWithholdingAccountNumberPlaceholder);
        company.LocalTaxAccountNumberPlaceholder = NormalizeIdentifier(company.LocalTaxAccountNumberPlaceholder);
        company.SutaState = NormalizeState(company.SutaState);

        if (PhoneNumberFormatter.TryFormat(company.Phone, out var formattedPhone, out _))
        {
            company.Phone = formattedPhone;
        }

        company.FutaRatePlaceholder = NormalizeRate(company.FutaRatePlaceholder);
        company.SutaRate = NormalizeRate(company.SutaRate);
        company.LocalEmployerTaxRate = NormalizeRate(company.LocalEmployerTaxRate);
    }

    private static string? NormalizeIdentifier(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string? NormalizeState(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToUpperInvariant();
    }

    private static decimal? NormalizeRate(decimal? value)
    {
        return value.HasValue ? Math.Round(value.Value, 4, MidpointRounding.AwayFromZero) : null;
    }
}
