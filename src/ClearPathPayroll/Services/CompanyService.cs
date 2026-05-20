using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;

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
            .ToListAsync();
    }

    /// <summary>
    /// Gets a company by ID.
    /// </summary>
    public async Task<Company?> GetCompanyByIdAsync(int id)
    {
        return await _context.Companies.FindAsync(id);
    }

    /// <summary>
    /// Creates a new company.
    /// </summary>
    public async Task<Company> CreateCompanyAsync(Company company)
    {
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
        company.UpdatedAt = DateTime.UtcNow;
        _context.Companies.Update(company);
        return await _context.SaveChangesAsync() > 0;
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
    /// Masks a FEIN for display purposes. Shows only first 2 digits and last 4 digits.
    /// Format: XX-XX-****
    /// Example: 12-3456789 -> 12-34-****
    /// </summary>
    /// <param name="fein">The unmasked FEIN (9 digits)</param>
    /// <returns>Masked FEIN string</returns>
    public static string MaskFEIN(string? fein)
    {
        if (string.IsNullOrEmpty(fein))
            return "XX-XX-****";

        var digits = fein.Replace("-", "").Trim();
        if (digits.Length < 5)
            return "XX-XX-****";

        // Show first 4 digits, mask the rest
        return $"{digits.Substring(0, 2)}-{digits.Substring(2, 2)}-****";
    }
}
