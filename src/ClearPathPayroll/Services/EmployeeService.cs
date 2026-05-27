using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Integrations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClearPathPayroll.Services;

/// <summary>
/// Service for managing employees.
/// </summary>
public class EmployeeService
{
    private readonly PayrollDbContext _context;
    private readonly IEncryptionService _encryptionService;

    public EmployeeService(PayrollDbContext context, IEncryptionService encryptionService)
    {
        _context = context;
        _encryptionService = encryptionService;
    }

    /// <summary>
    /// Gets all active employees for a company.
    /// </summary>
    public async Task<List<Employee>> GetActiveEmployeesByCompanyAsync(int companyId)
    {
        return await _context.Employees
            .Where(e => e.CompanyId == companyId && e.EmploymentStatus == EmploymentStatus.Active)
            .Include(e => e.Company)
            .ToListAsync();
    }

    public async Task<List<Employee>> GetEmployeesByCompanyAsync(int companyId)
    {
        return await _context.Employees
            .AsNoTracking()
            .Where(e => e.CompanyId == companyId)
            .Include(e => e.BankAccounts)
            .Include(e => e.PayrollFields)
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync();
    }

    /// <summary>
    /// Gets an employee by ID.
    /// </summary>
    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.Company)
            .Include(e => e.BankAccounts)
            .Include(e => e.PayrollFields)
            .FirstOrDefaultAsync(e => e.EmployeeId == id);
    }

    /// <summary>
    /// Creates a new employee.
    /// </summary>
    public async Task<Employee> CreateEmployeeAsync(Employee employee)
    {
        // Encrypt SSN if provided (placeholder)
        if (!string.IsNullOrEmpty(employee.FullSSNEncryptedPlaceholder))
        {
            employee.FullSSNEncryptedPlaceholder = await _encryptionService.EncryptAsync(employee.FullSSNEncryptedPlaceholder);
        }

        employee.CreatedAt = DateTime.UtcNow;
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee> SaveEmployeePayrollProfileAsync(Employee employee)
    {
        ValidateEmployeePayrollProfile(employee);
        employee.FullSSNEncryptedPlaceholder = null;
        employee.ResidenceAddress = string.IsNullOrWhiteSpace(employee.Address1)
            ? employee.ResidenceAddress
            : employee.Address1;

        if (employee.EmployeeId == 0)
        {
            employee.CreatedAt = DateTime.UtcNow;
            employee.PayrollProfileCreatedAt = DateTime.UtcNow;
            _context.Employees.Add(employee);
        }
        else
        {
            var existing = await _context.Employees
                .Include(e => e.BankAccounts)
                .Include(e => e.PayrollFields)
                .FirstOrDefaultAsync(e => e.EmployeeId == employee.EmployeeId);

            if (existing == null)
            {
                throw new InvalidOperationException($"Employee {employee.EmployeeId} was not found.");
            }

            _context.EmployeeBankAccounts.RemoveRange(existing.BankAccounts);
            _context.EmployeePayrollFields.RemoveRange(existing.PayrollFields);
            _context.Entry(existing).CurrentValues.SetValues(employee);
            existing.FullSSNEncryptedPlaceholder = null;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.PayrollProfileUpdatedAt = DateTime.UtcNow;

            foreach (var account in employee.BankAccounts.Take(5))
            {
                account.EmployeeBankAccountId = 0;
                account.EmployeeId = existing.EmployeeId;
                account.UpdatedAt = DateTime.UtcNow;
                existing.BankAccounts.Add(account);
            }

            foreach (var field in employee.PayrollFields.Where(f => !string.IsNullOrWhiteSpace(f.FieldName)))
            {
                field.EmployeePayrollFieldId = 0;
                field.EmployeeId = existing.EmployeeId;
                field.UpdatedAt = DateTime.UtcNow;
                existing.PayrollFields.Add(field);
            }

            await _context.SaveChangesAsync();
            return existing;
        }

        await _context.SaveChangesAsync();
        return employee;
    }

    private static void ValidateEmployeePayrollProfile(Employee employee)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(employee, new ValidationContext(employee), results, validateAllProperties: true);

        foreach (var account in employee.BankAccounts)
        {
            Validator.TryValidateObject(account, new ValidationContext(account), results, validateAllProperties: true);
        }

        results.AddRange(EmployeePayrollProfileValidator.ValidateDirectDepositAccounts(employee.BankAccounts));

        if (results.Any())
        {
            throw new ValidationException(string.Join(" ", results.Select(r => r.ErrorMessage)));
        }
    }

    /// <summary>
    /// Updates an existing employee.
    /// </summary>
    public async Task<bool> UpdateEmployeeAsync(Employee employee)
    {
        employee.UpdatedAt = DateTime.UtcNow;
        _context.Employees.Update(employee);
        return await _context.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Terminates an employee.
    /// </summary>
    public async Task<bool> TerminateEmployeeAsync(int id, DateTime terminationDate)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return false;

        employee.EmploymentStatus = EmploymentStatus.Terminated;
        employee.TerminationDate = terminationDate;
        employee.UpdatedAt = DateTime.UtcNow;
        _context.Employees.Update(employee);
        return await _context.SaveChangesAsync() > 0;
    }
}
