using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;

namespace ClearPathPayroll.Services;

public class PaycheckPrintService
{
    private readonly PayrollDbContext _context;

    public PaycheckPrintService(PayrollDbContext context)
    {
        _context = context;
    }

    public async Task<PrintablePaycheckModel> GetPrintablePaycheckAsync(int payrollRunEmployeeId)
    {
        var check = await _context.PayrollRunEmployees
            .Include(pre => pre.Employee)
            .Include(pre => pre.PayrollRun)
                .ThenInclude(run => run!.Company)
            .Include(pre => pre.NetPayLines)
            .FirstOrDefaultAsync(pre => pre.PayrollRunEmployeeId == payrollRunEmployeeId);

        if (check == null)
        {
            throw new InvalidOperationException("Payroll check was not found.");
        }

        if (check.NetPayLines.Any(line => string.Equals(line.PaymentMethod, "DirectDeposit", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Printable paychecks are available only for employees not paid by direct deposit.");
        }

        var employee = check.Employee ?? throw new InvalidOperationException("Employee data was not found for this payroll check.");
        var payrollRun = check.PayrollRun ?? throw new InvalidOperationException("Payroll run data was not found for this payroll check.");
        var company = payrollRun.Company ?? throw new InvalidOperationException("Company data was not found for this payroll check.");

        return new PrintablePaycheckModel
        {
            PayrollRunEmployeeId = payrollRunEmployeeId,
            CompanyName = company.LegalName,
            CompanyAddress = FormatAddress(company.PrimaryAddress, company.City, company.State, company.ZipCode),
            EmployeeName = $"{employee.FirstName} {employee.LastName}".Trim(),
            EmployeeAddress = FormatAddress(employee.Address1 ?? employee.ResidenceAddress, employee.City, employee.State, employee.ZipCode),
            PayDate = payrollRun.PayDate,
            PayPeriodStart = payrollRun.PayPeriodStart,
            PayPeriodEnd = payrollRun.PayPeriodEnd,
            NetPayAmount = check.NetPay,
            NetPayInWords = ConvertMoneyToWords(check.NetPay),
            MemoLine = $"Payroll run #{payrollRun.PayrollRunId}",
            CheckNumberPlaceholder = $"CHK-{payrollRun.PayrollRunId}-{check.EmployeeId}",
            IsDirectDeposit = false
        };
    }

    public static string ConvertMoneyToWords(decimal amount)
    {
        if (amount < 0 || amount > 999_999_999.99m)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount is outside the supported printable range.");
        }

        var dollars = (long)Math.Floor(amount);
        var cents = (int)Math.Round((amount - dollars) * 100m, 0);
        if (cents == 100)
        {
            dollars += 1;
            cents = 0;
        }

        return $"{CapitalizeFirstLetter(ConvertWholeNumberToWords(dollars))} and {cents:00}/100 dollars";
    }

    private static string FormatAddress(string? address, string? city, string? state, string? zipCode)
    {
        var location = string.Join(", ", new[] { city, state }.Where(part => !string.IsNullOrWhiteSpace(part)));
        var cityStateZip = string.Join(" ", new[] { location, zipCode }.Where(part => !string.IsNullOrWhiteSpace(part)));
        return string.Join(Environment.NewLine, new[] { address, cityStateZip }.Where(part => !string.IsNullOrWhiteSpace(part)));
    }

    private static string ConvertWholeNumberToWords(long number)
    {
        if (number == 0)
        {
            return "Zero";
        }

        var parts = new List<string>();
        AddScale(parts, number / 1_000_000, "million");
        number %= 1_000_000;
        AddScale(parts, number / 1_000, "thousand");
        number %= 1_000;

        if (number > 0)
        {
            parts.Add(ConvertUnderThousand(number));
        }

        return string.Join(" ", parts);
    }

    private static void AddScale(List<string> parts, long value, string scale)
    {
        if (value > 0)
        {
            parts.Add($"{ConvertUnderThousand(value)} {scale}");
        }
    }

    private static string ConvertUnderThousand(long number)
    {
        string[] small =
        [
            "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
            "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen",
            "seventeen", "eighteen", "nineteen"
        ];
        string[] tens = ["", "", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety"];

        var words = new List<string>();
        if (number >= 100)
        {
            words.Add($"{small[number / 100]} hundred");
            number %= 100;
        }

        if (number >= 20)
        {
            var ten = tens[number / 10];
            var remainder = number % 10;
            words.Add(remainder == 0 ? ten : $"{ten}-{small[remainder]}");
        }
        else if (number > 0)
        {
            words.Add(small[number]);
        }

        return string.Join(" ", words);
    }

    private static string CapitalizeFirstLetter(string value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? value
            : char.ToUpperInvariant(value[0]) + value[1..];
    }
}

public class PrintablePaycheckModel
{
    public int PayrollRunEmployeeId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyAddress { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeAddress { get; set; } = string.Empty;
    public DateTime PayDate { get; set; }
    public string CheckNumberPlaceholder { get; set; } = string.Empty;
    public decimal NetPayAmount { get; set; }
    public string NetPayInWords { get; set; } = string.Empty;
    public string MemoLine { get; set; } = string.Empty;
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public bool IsDirectDeposit { get; set; }
}
