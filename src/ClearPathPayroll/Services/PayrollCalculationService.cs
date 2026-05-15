using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using ClearPathPayroll.Integrations;
using Microsoft.EntityFrameworkCore;

namespace ClearPathPayroll.Services;

/// <summary>
/// Summary of a payroll run status and completeness.
/// </summary>
public class PayrollRunSummary
{
    /// <summary>
    /// Payroll run ID.
    /// </summary>
    public int PayrollRunId { get; set; }

    /// <summary>
    /// Company name.
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the payroll run.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Pay period start date.
    /// </summary>
    public DateTime PayPeriodStart { get; set; }

    /// <summary>
    /// Pay period end date.
    /// </summary>
    public DateTime PayPeriodEnd { get; set; }

    /// <summary>
    /// Actual pay date.
    /// </summary>
    public DateTime PayDate { get; set; }

    /// <summary>
    /// Total gross pay for the run.
    /// </summary>
    public decimal TotalGrossPay { get; set; }

    /// <summary>
    /// Total employee taxes.
    /// </summary>
    public decimal TotalEmployeeTaxes { get; set; }

    /// <summary>
    /// Total employer taxes.
    /// </summary>
    public decimal TotalEmployerTaxes { get; set; }

    /// <summary>
    /// Total deductions.
    /// </summary>
    public decimal TotalDeductions { get; set; }

    /// <summary>
    /// Total net pay.
    /// </summary>
    public decimal TotalNetPay { get; set; }

    /// <summary>
    /// Number of employees with calculated payroll.
    /// </summary>
    public int ProcessedEmployeeCount { get; set; }

    /// <summary>
    /// Number of active employees still pending processing.
    /// </summary>
    public int PendingEmployeeCount { get; set; }

    /// <summary>
    /// Total active employees in the company.
    /// </summary>
    public int TotalActiveEmployees { get; set; }

    /// <summary>
    /// Whether all active employees have been processed.
    /// </summary>
    public bool IsComplete { get; set; }
}

/// <summary>
/// Result of a complete payroll calculation for an employee.
/// </summary>
public class PayrollCalculationLineResult
{
    /// <summary>
    /// Gross pay.
    /// </summary>
    public decimal GrossPay { get; set; }

    /// <summary>
    /// Pre-tax deductions.
    /// </summary>
    public decimal PreTaxDeductions { get; set; }

    /// <summary>
    /// Taxable wages (gross - pre-tax deductions).
    /// </summary>
    public decimal TaxableWages { get; set; }

    /// <summary>
    /// Federal income tax.
    /// </summary>
    public decimal FederalIncomeTax { get; set; }

    /// <summary>
    /// State income tax.
    /// </summary>
    public decimal StateIncomeTax { get; set; }

    /// <summary>
    /// Social Security tax (employee).
    /// </summary>
    public decimal SocialSecurityTax { get; set; }

    /// <summary>
    /// Medicare tax (employee).
    /// </summary>
    public decimal MedicareTax { get; set; }

    /// <summary>
    /// Total employee taxes.
    /// </summary>
    public decimal TotalEmployeeTaxes { get; set; }

    /// <summary>
    /// Post-tax deductions.
    /// </summary>
    public decimal PostTaxDeductions { get; set; }

    /// <summary>
    /// Net pay.
    /// </summary>
    public decimal NetPay { get; set; }

    /// <summary>
    /// Employer Social Security tax.
    /// </summary>
    public decimal EmployerSocialSecurityTax { get; set; }

    /// <summary>
    /// Employer Medicare tax.
    /// </summary>
    public decimal EmployerMedicareTax { get; set; }

    /// <summary>
    /// Total employer taxes.
    /// </summary>
    public decimal TotalEmployerTaxes { get; set; }

    /// <summary>
    /// Detailed earning lines.
    /// </summary>
    public List<(string Description, decimal Amount)> EarningLines { get; set; } = new();

    /// <summary>
    /// Detailed deduction lines.
    /// </summary>
    public List<(string Description, string Type, decimal Amount)> DeductionLines { get; set; } = new();

    /// <summary>
    /// Detailed tax lines.
    /// </summary>
    public List<(string TaxType, decimal Amount)> TaxLines { get; set; } = new();
}

/// <summary>
/// Service for orchestrating complete payroll calculations.
/// </summary>
public class PayrollCalculationService
{
    private readonly PayrollDbContext _context;
    private readonly GrossPayCalculationService _grossPayService;
    private readonly DeductionCalculationService _deductionService;
    private readonly ITaxCalculationService _taxService;

    public PayrollCalculationService(
        PayrollDbContext context,
        GrossPayCalculationService grossPayService,
        DeductionCalculationService deductionService,
        ITaxCalculationService taxService)
    {
        _context = context;
        _grossPayService = grossPayService;
        _deductionService = deductionService;
        _taxService = taxService;
    }

    private async Task AddAuditLogAsync(int payrollRunId, string eventType, string description, string createdByUserId)
    {
        _context.AuditLogEntries.Add(new AuditLogEntry
        {
            PayrollRunId = payrollRunId,
            EventType = eventType,
            Description = description,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Calculates complete payroll for an employee in a payroll run.
    /// </summary>
    public async Task<PayrollCalculationLineResult> CalculatePayrollForEmployeeAsync(
        Employee employee,
        PaySchedule paySchedule,
        List<DeductionInput> deductions,
        decimal? hoursWorked = null)
    {
        var result = new PayrollCalculationLineResult();

        // 1. Calculate gross pay
        var grossPayResult = _grossPayService.CalculateGrossPay(employee, paySchedule, hoursWorked);
        result.GrossPay = grossPayResult.TotalGrossPay;

        // Add earning lines
        if (grossPayResult.RegularHours > 0 || grossPayResult.OvertimeHours > 0)
        {
            if (grossPayResult.RegularHours > 0)
            {
                result.EarningLines.Add(("Regular Pay", grossPayResult.RegularPay));
            }
            if (grossPayResult.OvertimeHours > 0)
            {
                result.EarningLines.Add(("Overtime Pay", grossPayResult.OvertimePay));
            }
        }
        else if (employee.PayType == PayType.Salary)
        {
            result.EarningLines.Add(("Salary", result.GrossPay));
        }

        // 2. Calculate pre-tax deductions
        var preTaxDeductions = deductions?
            .Where(d => d.Type == DeductionType.PreTaxFixedAmount || d.Type == DeductionType.PreTaxPercentage)
            .ToList() ?? new();

        var preTaxResult = _deductionService.CalculateDeductions(result.GrossPay, 0, preTaxDeductions);
        result.PreTaxDeductions = preTaxResult.TotalPreTaxDeductions;

        foreach (var line in preTaxResult.DeductionLines)
        {
            result.DeductionLines.Add((line.Description, "Pre-Tax", line.Amount));
        }

        // 3. Calculate taxable wages and taxes
        result.TaxableWages = Math.Round(result.GrossPay - result.PreTaxDeductions, 2);
        
        var taxResult = await _taxService.CalculateTaxesAsync(result.TaxableWages, employee.State, employee.State);
        result.FederalIncomeTax = taxResult.FederalIncomeTax;
        result.StateIncomeTax = taxResult.StateIncomeTax;
        result.SocialSecurityTax = taxResult.SocialSecurityTax;
        result.MedicareTax = taxResult.MedicareTax;
        result.TotalEmployeeTaxes = taxResult.TotalEmployeeTaxes;
        result.EmployerSocialSecurityTax = taxResult.EmployerSocialSecurityTax;
        result.EmployerMedicareTax = taxResult.EmployerMedicareTax;
        result.TotalEmployerTaxes = taxResult.TotalEmployerTaxes;

        // Add tax lines
        result.TaxLines.Add(("Federal Income Tax", result.FederalIncomeTax));
        result.TaxLines.Add(("State Income Tax", result.StateIncomeTax));
        result.TaxLines.Add(("Social Security Tax", result.SocialSecurityTax));
        result.TaxLines.Add(("Medicare Tax", result.MedicareTax));

        // 4. Calculate net pay before post-tax deductions
        var netPayBeforePostTax = Math.Round(result.GrossPay - result.PreTaxDeductions - result.TotalEmployeeTaxes, 2);

        // 5. Calculate post-tax deductions
        var postTaxDeductions = deductions?
            .Where(d => d.Type == DeductionType.PostTaxFixedAmount || d.Type == DeductionType.PostTaxPercentage)
            .ToList() ?? new();

        var postTaxResult = _deductionService.CalculateDeductions(result.GrossPay, netPayBeforePostTax, postTaxDeductions);
        result.PostTaxDeductions = postTaxResult.TotalPostTaxDeductions;

        foreach (var line in postTaxResult.DeductionLines)
        {
            result.DeductionLines.Add((line.Description, "Post-Tax", line.Amount));
        }

        // 6. Calculate final net pay
        result.NetPay = Math.Round(netPayBeforePostTax - result.PostTaxDeductions, 2);

        // 7. Prevent negative net pay (should not happen with proper deduction capping)
        if (result.NetPay < 0)
        {
            result.NetPay = 0;
        }

        return result;
    }

    /// <summary>
    /// Saves calculation results to the database as line items.
    /// </summary>
    public async Task SavePayrollCalculationAsync(
        int payrollRunId,
        int employeeId,
        PayrollCalculationLineResult calculation)
    {
        var payrollRun = await _context.PayrollRuns
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.EarningLines)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.DeductionLines)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.TaxLines)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.NetPayLines)
            .FirstOrDefaultAsync(p => p.PayrollRunId == payrollRunId);

        if (payrollRun == null)
        {
            throw new InvalidOperationException($"Payroll run {payrollRunId} not found");
        }

        // Create or update payroll run employee
        var payrollRunEmployee = payrollRun.PayrollRunEmployees
            .FirstOrDefault(pre => pre.EmployeeId == employeeId);

        if (payrollRunEmployee == null)
        {
            payrollRunEmployee = new PayrollRunEmployee
            {
                PayrollRunId = payrollRunId,
                EmployeeId = employeeId
            };
            _context.PayrollRunEmployees.Add(payrollRunEmployee);
        }

        // Update aggregates
        payrollRunEmployee.GrossPay = calculation.GrossPay;
        payrollRunEmployee.TotalDeductions = calculation.PreTaxDeductions + calculation.PostTaxDeductions;
        payrollRunEmployee.TotalTaxes = calculation.TotalEmployeeTaxes;
        payrollRunEmployee.NetPay = calculation.NetPay;

        // Clear existing lines
        payrollRunEmployee.EarningLines.Clear();
        payrollRunEmployee.DeductionLines.Clear();
        payrollRunEmployee.TaxLines.Clear();
        payrollRunEmployee.NetPayLines.Clear();

        // Add earning lines
        foreach (var (description, amount) in calculation.EarningLines)
        {
            payrollRunEmployee.EarningLines.Add(new EarningLine
            {
                Description = description,
                Amount = amount
            });
        }

        // Add deduction lines
        foreach (var (description, type, amount) in calculation.DeductionLines)
        {
            payrollRunEmployee.DeductionLines.Add(new DeductionLine
            {
                Description = description,
                Amount = amount
            });
        }

        // Add tax lines
        foreach (var (taxType, amount) in calculation.TaxLines)
        {
            payrollRunEmployee.TaxLines.Add(new TaxLine
            {
                TaxType = taxType,
                Amount = amount
            });
        }

        // Add net pay line
        payrollRunEmployee.NetPayLines.Add(new NetPayLine
        {
            Amount = calculation.NetPay,
            PaymentMethod = "DirectDeposit" // Placeholder
        });

        // Remove and recreate employer tax lines for this employee
        var existingEmployerTaxLines = await _context.EmployerTaxLines
            .Where(e => e.PayrollRunId == payrollRunId && e.EmployeeId == employeeId)
            .ToListAsync();

        if (existingEmployerTaxLines.Any())
        {
            _context.EmployerTaxLines.RemoveRange(existingEmployerTaxLines);
        }

        if (calculation.EmployerSocialSecurityTax > 0)
        {
            _context.EmployerTaxLines.Add(new EmployerTaxLine
            {
                PayrollRunId = payrollRunId,
                EmployeeId = employeeId,
                TaxType = "Employer Social Security Tax",
                Amount = calculation.EmployerSocialSecurityTax
            });
        }

        if (calculation.EmployerMedicareTax > 0)
        {
            _context.EmployerTaxLines.Add(new EmployerTaxLine
            {
                PayrollRunId = payrollRunId,
                EmployeeId = employeeId,
                TaxType = "Employer Medicare Tax",
                Amount = calculation.EmployerMedicareTax
            });
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Calculates and saves payroll for an employee with optional pay period override and manual gross pay adjustment.
    /// Supports both live and retroactive/after-the-fact payroll processing.
    /// </summary>
    public async Task<PayrollCalculationLineResult> CalculateAndSavePayrollForEmployeeAsync(
        int payrollRunId,
        int employeeId,
        List<DeductionInput> deductions,
        decimal? hoursWorked = null,
        string performedByUserId = "system",
        DateTime? overridePayPeriodStartDate = null,
        DateTime? overridePayPeriodEndDate = null,
        decimal? manualGrossPayAdjustment = null)
    {
        var payrollRun = await _context.PayrollRuns
            .Include(p => p.PaySchedule)
            .Include(p => p.PayrollRunEmployees)
            .FirstOrDefaultAsync(p => p.PayrollRunId == payrollRunId);

        if (payrollRun == null)
        {
            throw new InvalidOperationException($"Payroll run {payrollRunId} not found.");
        }

        if (payrollRun.PaySchedule == null)
        {
            throw new InvalidOperationException($"Payroll run {payrollRunId} does not have an associated pay schedule.");
        }

        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        if (employee == null)
        {
            throw new InvalidOperationException($"Employee {employeeId} not found.");
        }

        var isRetroactive = overridePayPeriodStartDate.HasValue || overridePayPeriodEndDate.HasValue;
        var auditEventType = isRetroactive ? "RetroactivePayrollCalculationStarted" : "PayrollCalculationStarted";
        
        await AddAuditLogAsync(payrollRunId,
            auditEventType,
            $"Starting payroll calculation for employee {employeeId} in payroll run {payrollRunId}. " +
            (isRetroactive ? $"Retroactive processing: {overridePayPeriodStartDate:yyyy-MM-dd} to {overridePayPeriodEndDate:yyyy-MM-dd}." : "Live processing."),
            performedByUserId);

        var calculation = await CalculatePayrollForEmployeeAsync(employee, payrollRun.PaySchedule, deductions, hoursWorked);
        
        // Apply manual gross pay adjustments if provided (e.g., bonuses, corrections)
        if (manualGrossPayAdjustment.HasValue && manualGrossPayAdjustment.Value != 0)
        {
            var adjustment = Math.Round(manualGrossPayAdjustment.Value, 2);
            calculation.GrossPay = Math.Round(calculation.GrossPay + adjustment, 2);
            calculation.EarningLines.Add(("Manual Adjustment", adjustment));
            
            await AddAuditLogAsync(payrollRunId,
                "ManualGrossPayAdjustment",
                $"Applied manual gross pay adjustment of {adjustment:C} for employee {employeeId}.",
                performedByUserId);
        }
        
        await SavePayrollCalculationAsync(payrollRunId, employeeId, calculation);
        await UpdatePayrollRunTotalsAsync(payrollRunId);

        auditEventType = isRetroactive ? "RetroactivePayrollCalculationCompleted" : "PayrollCalculationCompleted";
        await AddAuditLogAsync(payrollRunId,
            auditEventType,
            $"Completed payroll calculation for employee {employeeId} in payroll run {payrollRunId}.",
            performedByUserId);

        return calculation;
    }

    /// <summary>
    /// Processes payroll for all eligible employees in a payroll run.
    /// Supports batch processing for both live and retroactive scenarios.
    /// </summary>
    public async Task<Dictionary<int, PayrollCalculationLineResult>> ProcessFullPayrollRunAsync(
        int payrollRunId,
        Dictionary<int, List<DeductionInput>>? employeeDeductions = null,
        Dictionary<int, decimal?>? hoursWorkedByEmployee = null,
        Dictionary<int, decimal?>? manualAdjustmentsByEmployee = null,
        string performedByUserId = "system")
    {
        var payrollRun = await _context.PayrollRuns
            .Include(p => p.PaySchedule)
            .Include(p => p.PayrollRunEmployees)
            .FirstOrDefaultAsync(p => p.PayrollRunId == payrollRunId);

        if (payrollRun == null)
        {
            throw new InvalidOperationException($"Payroll run {payrollRunId} not found.");
        }

        if (payrollRun.PaySchedule == null)
        {
            throw new InvalidOperationException($"Payroll run {payrollRunId} does not have an associated pay schedule.");
        }

        // Get all employees for the company
        var employees = await _context.Employees
            .Where(e => e.CompanyId == payrollRun.CompanyId && e.EmploymentStatus == EmploymentStatus.Active)
            .ToListAsync();

        if (!employees.Any())
        {
            throw new InvalidOperationException($"No active employees found for company {payrollRun.CompanyId}.");
        }

        await AddAuditLogAsync(payrollRunId,
            "FullPayrollProcessingStarted",
            $"Starting batch payroll processing for {employees.Count} employees in payroll run {payrollRunId}.",
            performedByUserId);

        var results = new Dictionary<int, PayrollCalculationLineResult>();

        foreach (var employee in employees)
        {
            var deductions = employeeDeductions?.ContainsKey(employee.EmployeeId) == true
                ? employeeDeductions[employee.EmployeeId]
                : new List<DeductionInput>();

            var hoursWorked = hoursWorkedByEmployee?.ContainsKey(employee.EmployeeId) == true
                ? hoursWorkedByEmployee[employee.EmployeeId]
                : null;

            var manualAdjustment = manualAdjustmentsByEmployee?.ContainsKey(employee.EmployeeId) == true
                ? manualAdjustmentsByEmployee[employee.EmployeeId]
                : null;

            try
            {
                var result = await CalculateAndSavePayrollForEmployeeAsync(
                    payrollRunId,
                    employee.EmployeeId,
                    deductions,
                    hoursWorked,
                    performedByUserId,
                    null,
                    null,
                    manualAdjustment);

                results[employee.EmployeeId] = result;
            }
            catch (Exception ex)
            {
                await AddAuditLogAsync(payrollRunId,
                    "PayrollCalculationFailed",
                    $"Failed to calculate payroll for employee {employee.EmployeeId}: {ex.Message}",
                    performedByUserId);

                throw;
            }
        }

        await AddAuditLogAsync(payrollRunId,
            "FullPayrollProcessingCompleted",
            $"Completed batch payroll processing for {results.Count} employees in payroll run {payrollRunId}.",
            performedByUserId);

        return results;
    }

    /// <summary>
    /// Allows payroll recalculation for an employee (useful for corrections or adjustments).
    /// Clears existing calculated data and recalculates from scratch.
    /// </summary>
    public async Task<PayrollCalculationLineResult> RecalculatePayrollForEmployeeAsync(
        int payrollRunId,
        int employeeId,
        List<DeductionInput> deductions,
        decimal? hoursWorked = null,
        string performedByUserId = "system",
        decimal? manualGrossPayAdjustment = null)
    {
        var payrollRunEmployee = await _context.PayrollRunEmployees
            .Include(pre => pre.EarningLines)
            .Include(pre => pre.DeductionLines)
            .Include(pre => pre.TaxLines)
            .Include(pre => pre.NetPayLines)
            .FirstOrDefaultAsync(pre => pre.PayrollRunId == payrollRunId && pre.EmployeeId == employeeId);

        if (payrollRunEmployee != null)
        {
            // Clear existing lines for recalculation
            _context.EarningLines.RemoveRange(payrollRunEmployee.EarningLines);
            _context.DeductionLines.RemoveRange(payrollRunEmployee.DeductionLines);
            _context.TaxLines.RemoveRange(payrollRunEmployee.TaxLines);
            _context.NetPayLines.RemoveRange(payrollRunEmployee.NetPayLines);
            await _context.SaveChangesAsync();

            await AddAuditLogAsync(payrollRunId,
                "PayrollRecalculationStarted",
                $"Starting recalculation for employee {employeeId}.",
                performedByUserId);
        }

        var result = await CalculateAndSavePayrollForEmployeeAsync(
            payrollRunId,
            employeeId,
            deductions,
            hoursWorked,
            performedByUserId,
            null,
            null,
            manualGrossPayAdjustment);

        await AddAuditLogAsync(payrollRunId,
            "PayrollRecalculationCompleted",
            $"Completed recalculation for employee {employeeId}.",
            performedByUserId);

        return result;
    }

    public async Task UpdatePayrollRunTotalsAsync(int payrollRunId)
    {
        var payrollRun = await _context.PayrollRuns
            .Include(p => p.PayrollRunEmployees)
            .Include(p => p.EmployerTaxLines)
            .FirstOrDefaultAsync(p => p.PayrollRunId == payrollRunId);

        if (payrollRun == null)
        {
            throw new InvalidOperationException($"Payroll run {payrollRunId} not found.");
        }

        payrollRun.TotalGrossPay = payrollRun.PayrollRunEmployees.Sum(e => e.GrossPay);
        payrollRun.TotalEmployeeTaxes = payrollRun.PayrollRunEmployees.Sum(e => e.TotalTaxes);
        payrollRun.TotalDeductions = payrollRun.PayrollRunEmployees.Sum(e => e.TotalDeductions);
        payrollRun.TotalNetPay = payrollRun.PayrollRunEmployees.Sum(e => e.NetPay);
        payrollRun.TotalEmployerTaxes = payrollRun.EmployerTaxLines.Sum(e => e.Amount);

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Validates whether a payroll run can be recalculated (must be in Draft or Calculated status).
    /// </summary>
    public async Task<bool> CanRecalculatePayrollRunAsync(int payrollRunId)
    {
        var payrollRun = await _context.PayrollRuns
            .FirstOrDefaultAsync(p => p.PayrollRunId == payrollRunId);

        if (payrollRun == null)
        {
            return false;
        }

        return payrollRun.Status == PayrollStatus.Draft || payrollRun.Status == PayrollStatus.Calculated;
    }

    /// <summary>
    /// Voids a payroll run and all its employee calculations. Creates audit trail.
    /// Useful for corrections and do-over scenarios in retroactive processing.
    /// </summary>
    public async Task VoidPayrollRunAsync(int payrollRunId, string reason, string performedByUserId = "system")
    {
        var payrollRun = await _context.PayrollRuns
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.EarningLines)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.DeductionLines)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.TaxLines)
            .Include(p => p.PayrollRunEmployees)
                .ThenInclude(pre => pre.NetPayLines)
            .Include(p => p.EmployerTaxLines)
            .FirstOrDefaultAsync(p => p.PayrollRunId == payrollRunId);

        if (payrollRun == null)
        {
            throw new InvalidOperationException($"Payroll run {payrollRunId} not found.");
        }

        // Can void Draft, Calculated, or Approved runs
        if (payrollRun.Status == PayrollStatus.Submitted || payrollRun.Status == PayrollStatus.Completed)
        {
            throw new InvalidOperationException($"Cannot void payroll run {payrollRunId} with status {payrollRun.Status}. Only Draft, Calculated, or Approved runs can be voided.");
        }

        // Clear all employee calculations
        foreach (var employee in payrollRun.PayrollRunEmployees)
        {
            _context.EarningLines.RemoveRange(employee.EarningLines);
            _context.DeductionLines.RemoveRange(employee.DeductionLines);
            _context.TaxLines.RemoveRange(employee.TaxLines);
            _context.NetPayLines.RemoveRange(employee.NetPayLines);

            employee.GrossPay = 0;
            employee.TotalDeductions = 0;
            employee.TotalTaxes = 0;
            employee.NetPay = 0;
        }

        // Clear employer tax lines
        _context.EmployerTaxLines.RemoveRange(payrollRun.EmployerTaxLines);

        // Reset totals
        payrollRun.TotalGrossPay = 0;
        payrollRun.TotalEmployeeTaxes = 0;
        payrollRun.TotalEmployerTaxes = 0;
        payrollRun.TotalDeductions = 0;
        payrollRun.TotalNetPay = 0;
        payrollRun.Status = PayrollStatus.Voided;

        await _context.SaveChangesAsync();

        await AddAuditLogAsync(payrollRunId,
            "PayrollRunVoided",
            $"Voided payroll run {payrollRunId}. Reason: {reason}",
            performedByUserId);
    }

    /// <summary>
    /// Gets a summary of the payroll run status and completeness.
    /// Useful for validation before submission or to check retroactive run status.
    /// </summary>
    public async Task<PayrollRunSummary> GetPayrollRunSummaryAsync(int payrollRunId)
    {
        var payrollRun = await _context.PayrollRuns
            .Include(p => p.PayrollRunEmployees)
            .Include(p => p.Company)
            .FirstOrDefaultAsync(p => p.PayrollRunId == payrollRunId);

        if (payrollRun == null)
        {
            throw new InvalidOperationException($"Payroll run {payrollRunId} not found.");
        }

        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.CompanyId == payrollRun.CompanyId);

        var activeEmployees = await _context.Employees
            .Where(e => e.CompanyId == payrollRun.CompanyId && e.EmploymentStatus == EmploymentStatus.Active)
            .ToListAsync();

        var processedCount = payrollRun.PayrollRunEmployees.Count;
        var pendingCount = activeEmployees.Count - processedCount;

        return new PayrollRunSummary
        {
            PayrollRunId = payrollRunId,
            CompanyName = company?.LegalName ?? "Unknown",
            Status = payrollRun.Status.ToString(),
            PayPeriodStart = payrollRun.PayPeriodStart,
            PayPeriodEnd = payrollRun.PayPeriodEnd,
            PayDate = payrollRun.PayDate,
            TotalGrossPay = payrollRun.TotalGrossPay,
            TotalEmployeeTaxes = payrollRun.TotalEmployeeTaxes,
            TotalEmployerTaxes = payrollRun.TotalEmployerTaxes,
            TotalDeductions = payrollRun.TotalDeductions,
            TotalNetPay = payrollRun.TotalNetPay,
            ProcessedEmployeeCount = processedCount,
            PendingEmployeeCount = pendingCount,
            TotalActiveEmployees = activeEmployees.Count,
            IsComplete = pendingCount == 0
        };
    }
}