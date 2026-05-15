using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Integrations;
using Microsoft.EntityFrameworkCore;

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

    /// <summary>
    /// Gets an employee by ID.
    /// </summary>
    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        return await _context.Employees
            .Include(e => e.Company)
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