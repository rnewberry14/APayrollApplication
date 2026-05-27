using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClearPathPayroll.Services;

public class PayrollItemService
{
    private readonly PayrollDbContext _context;

    public PayrollItemService(PayrollDbContext context)
    {
        _context = context;
    }

    public async Task<List<PayrollItem>> GetPayrollItemsByCompanyAsync(int companyId)
    {
        return await _context.PayrollItems
            .Where(item => item.CompanyId == companyId)
            .OrderBy(item => item.ItemCode)
            .ToListAsync();
    }

    public async Task<PayrollItem?> GetPayrollItemByIdAsync(int payrollItemId)
    {
        return await _context.PayrollItems.FindAsync(payrollItemId);
    }

    public async Task<PayrollItem> SavePayrollItemAsync(PayrollItem item)
    {
        ValidatePayrollItem(item);

        if (item.PayrollItemId == 0)
        {
            item.CreatedAt = DateTime.UtcNow;
            _context.PayrollItems.Add(item);
        }
        else
        {
            item.UpdatedAt = DateTime.UtcNow;
            _context.PayrollItems.Update(item);
        }

        await _context.SaveChangesAsync();
        return item;
    }

    private static void ValidatePayrollItem(PayrollItem item)
    {
        var context = new ValidationContext(item);
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(item, context, results, true))
        {
            throw new ValidationException(results[0].ErrorMessage);
        }
    }
}
