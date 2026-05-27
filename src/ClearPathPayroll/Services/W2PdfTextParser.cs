using ClearPathPayroll.Domain;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ClearPathPayroll.Services;

public class W2PdfTextParser
{
    private static readonly Regex SsnRegex = new(@"\b\d{3}-?\d{2}-?\d{4}\b", RegexOptions.Compiled);
    private static readonly Regex EinRegex = new(@"\b\d{2}-?\d{7}\b", RegexOptions.Compiled);
    private static readonly Regex TaxYearRegex = new(@"\b(19|20)\d{2}\b", RegexOptions.Compiled);

    public W2PdfParseResult Parse(string extractedText)
    {
        var result = new W2PdfParseResult();
        var record = new W2ImportRecord
        {
            RawExtractedTextPreview = BuildSafePreview(extractedText),
            NeedsManualReview = true
        };

        if (string.IsNullOrWhiteSpace(extractedText))
        {
            result.Records.Add(record);
            result.Errors.Add(CreateWarning("PdfText", "No selectable text was extracted. The PDF may be scanned or image-only."));
            record.ConfidenceScore = 0m;
            return result;
        }

        var normalized = NormalizeWhitespace(extractedText);
        record.TaxYear = ParseTaxYear(normalized);
        record.EmployerName = FirstTextMatch(normalized, @"Employer(?:'s)?\s+name\s*[:\-]?\s*(?<value>.+?)(?=\s+(?:Employer(?:'s)?\s+address|Employee(?:'s)?\s+name|EIN|Federal|Box|1\s+Wages)|$)");
        record.EmployerAddress = FirstTextMatch(normalized, @"Employer(?:'s)?\s+address\s*[:\-]?\s*(?<value>.+?)(?=\s+(?:Employee(?:'s)?\s+name|Employee(?:'s)?\s+address|Box|1\s+Wages)|$)");
        record.EmployeeAddress = FirstTextMatch(normalized, @"Employee(?:'s)?\s+address\s*[:\-]?\s*(?<value>.+?)(?=\s+(?:Box|1\s+Wages|Control number)|$)");
        ApplyEmployeeName(record, FirstTextMatch(normalized, @"Employee(?:'s)?\s+name\s*[:\-]?\s*(?<value>.+?)(?=\s+(?:Employee(?:'s)?\s+address|SSN|Social security|Box|1\s+Wages)|$)"));

        record.EmployeeSSNLast4Only = Last4(SsnRegex.Match(normalized).Value);
        record.EmployerEINLast4Only = Last4(FindEmployerEin(normalized));

        record.Box1WagesTipsOtherCompensation = ParseBoxAmount(normalized, "1", "Wages, tips, other compensation");
        record.Box2FederalIncomeTaxWithheld = ParseBoxAmount(normalized, "2", "Federal income tax withheld");
        record.Box3SocialSecurityWages = ParseBoxAmount(normalized, "3", "Social security wages");
        record.Box4SocialSecurityTaxWithheld = ParseBoxAmount(normalized, "4", "Social security tax withheld");
        record.Box5MedicareWagesAndTips = ParseBoxAmount(normalized, "5", "Medicare wages and tips");
        record.Box6MedicareTaxWithheld = ParseBoxAmount(normalized, "6", "Medicare tax withheld");
        record.Box7SocialSecurityTips = ParseBoxAmount(normalized, "7", "Social security tips");
        record.Box8AllocatedTips = ParseBoxAmount(normalized, "8", "Allocated tips");
        record.Box10DependentCareBenefits = ParseBoxAmount(normalized, "10", "Dependent care benefits");
        record.Box11NonqualifiedPlans = ParseBoxAmount(normalized, "11", "Nonqualified plans");

        record.State1 = FirstTextMatch(normalized, @"\bState\s*[:\-]?\s*(?<value>[A-Z]{2})\b");
        record.EmployerStateId1Masked = MaskIdentifier(FirstTextMatch(normalized, @"Employer(?:'s)?\s+state\s+ID(?:\s+number)?\s*[:\-]?\s*(?<value>[A-Za-z0-9\-]+)"));
        record.StateWages1 = ParseBoxAmount(normalized, "16", "State wages, tips, etc.");
        record.StateIncomeTax1 = ParseBoxAmount(normalized, "17", "State income tax");
        record.LocalWages1 = ParseBoxAmount(normalized, "18", "Local wages, tips, etc.");
        record.LocalIncomeTax1 = ParseBoxAmount(normalized, "19", "Local income tax");
        record.LocalityName1 = FirstTextMatch(normalized, @"20\s+Locality\s+name\s*[:\-]?\s*(?<value>[A-Za-z0-9 \-]+?)(?=\s+(?:State|$))");

        AddRecordWarnings(record, result.Errors);
        record.ConfidenceScore = CalculateConfidence(record);
        record.NeedsManualReview = record.ConfidenceScore < 0.85m || result.Errors.Any(error => error.Severity != W2ImportErrorSeverity.Info);
        result.Records.Add(record);
        return result;
    }

    public static string BuildSafePreview(string extractedText)
    {
        if (string.IsNullOrWhiteSpace(extractedText))
        {
            return string.Empty;
        }

        var preview = extractedText.Length > 2000 ? extractedText[..2000] : extractedText;
        preview = SsnRegex.Replace(preview, match => $"***-**-{Last4(match.Value)}");
        preview = EinRegex.Replace(preview, match => $"**-***{Last4(match.Value)}");
        return preview;
    }

    public static string Last4(string? value)
    {
        var digits = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
        return digits.Length >= 4 ? digits[^4..] : string.Empty;
    }

    public static string MaskIdentifier(string? value)
    {
        var digits = new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
        return digits.Length >= 4 ? $"masked-{digits[^4..]}" : string.Empty;
    }

    private static void ApplyEmployeeName(W2ImportRecord record, string employeeName)
    {
        if (string.IsNullOrWhiteSpace(employeeName))
        {
            return;
        }

        var parts = employeeName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 1)
        {
            record.EmployeeFirstName = parts[0];
            return;
        }

        record.EmployeeFirstName = parts[0];
        record.EmployeeLastName = parts[^1];
        if (parts.Length > 2)
        {
            record.EmployeeMiddleInitial = parts[1][0].ToString(CultureInfo.InvariantCulture);
        }
    }

    private static int? ParseTaxYear(string text)
    {
        var matches = TaxYearRegex.Matches(text)
            .Select(match => int.Parse(match.Value, CultureInfo.InvariantCulture))
            .Where(year => year >= 1900 && year <= DateTime.Today.Year + 1)
            .ToList();

        return matches.Count == 0 ? null : matches.Max();
    }

    private static decimal? ParseBoxAmount(string text, string boxNumber, string label)
    {
        var escapedLabel = Regex.Escape(label).Replace(@"\ ", @"\s+");
        var patterns = new[]
        {
            $@"(?:box\s*)?{boxNumber}\s+{escapedLabel}\s*[:\-]?\s*\$?(?<value>-?\(?\d[\d,]*(?:\.\d{{1,2}})?\)?)",
            $@"{escapedLabel}\s*[:\-]?\s*\$?(?<value>-?\(?\d[\d,]*(?:\.\d{{1,2}})?\)?)"
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            if (match.Success && TryParseMoney(match.Groups["value"].Value, out var amount))
            {
                return amount;
            }
        }

        return null;
    }

    private static bool TryParseMoney(string value, out decimal amount)
    {
        var normalized = value.Trim();
        var negative = normalized.StartsWith('(') && normalized.EndsWith(')');
        normalized = normalized.Trim('(', ')');
        if (decimal.TryParse(normalized, NumberStyles.Currency, CultureInfo.InvariantCulture, out amount))
        {
            if (negative)
            {
                amount *= -1;
            }

            return true;
        }

        return false;
    }

    private static string FindEmployerEin(string text)
    {
        var match = Regex.Match(text, @"(?:EIN|Employer(?:'s)?\s+identification\s+number)\s*[:\-]?\s*(?<value>\d{2}-?\d{7})", RegexOptions.IgnoreCase);
        return match.Success ? match.Groups["value"].Value : string.Empty;
    }

    private static string FirstTextMatch(string text, string pattern)
    {
        var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
        return match.Success ? CleanText(match.Groups["value"].Value) : string.Empty;
    }

    private static string CleanText(string value)
    {
        return Regex.Replace(value.Trim(), @"\s{2,}", " ");
    }

    private static string NormalizeWhitespace(string text)
    {
        return Regex.Replace(text.Replace("\r", " ").Replace("\n", " "), @"\s+", " ").Trim();
    }

    private static void AddRecordWarnings(W2ImportRecord record, List<W2ImportError> errors)
    {
        if (record.TaxYear == null)
        {
            errors.Add(CreateError("TaxYear", "TaxYear is required."));
        }

        if (string.IsNullOrWhiteSpace(record.EmployeeFirstName) || string.IsNullOrWhiteSpace(record.EmployeeLastName))
        {
            errors.Add(CreateError("EmployeeName", "Employee name is required."));
        }

        if (string.IsNullOrWhiteSpace(record.EmployerName))
        {
            errors.Add(CreateError("EmployerName", "Employer name is required."));
        }

        if (string.IsNullOrWhiteSpace(record.EmployeeSSNLast4Only))
        {
            errors.Add(CreateWarning("EmployeeSSNLast4Only", "Employee SSN last 4 was not extracted. Review and enter last 4 if available."));
        }

        if (!HasAnyAmount(record))
        {
            errors.Add(CreateError("Amounts", "At least one wage or tax amount is required."));
        }

        if (GetAmountValues(record).Any(amount => amount < 0))
        {
            errors.Add(CreateWarning("Amounts", "One or more extracted amounts are negative. Review before saving."));
        }
    }

    private static decimal CalculateConfidence(W2ImportRecord record)
    {
        var checks = new[]
        {
            record.TaxYear != null,
            !string.IsNullOrWhiteSpace(record.EmployerName),
            !string.IsNullOrWhiteSpace(record.EmployeeFirstName),
            !string.IsNullOrWhiteSpace(record.EmployeeLastName),
            !string.IsNullOrWhiteSpace(record.EmployeeSSNLast4Only),
            HasAnyAmount(record)
        };

        return checks.Count(check => check) / (decimal)checks.Length;
    }

    private static bool HasAnyAmount(W2ImportRecord record)
    {
        return GetAmountValues(record).Any();
    }

    private static IEnumerable<decimal> GetAmountValues(W2ImportRecord record)
    {
        return new decimal?[]
        {
            record.Box1WagesTipsOtherCompensation,
            record.Box2FederalIncomeTaxWithheld,
            record.Box3SocialSecurityWages,
            record.Box4SocialSecurityTaxWithheld,
            record.Box5MedicareWagesAndTips,
            record.Box6MedicareTaxWithheld,
            record.StateWages1,
            record.StateIncomeTax1,
            record.LocalWages1,
            record.LocalIncomeTax1
        }.Where(amount => amount.HasValue).Select(amount => amount!.Value);
    }

    private static W2ImportError CreateWarning(string fieldName, string message)
    {
        return new W2ImportError
        {
            Severity = W2ImportErrorSeverity.Warning,
            FieldName = fieldName,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static W2ImportError CreateError(string fieldName, string message)
    {
        return new W2ImportError
        {
            Severity = W2ImportErrorSeverity.Error,
            FieldName = fieldName,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };
    }
}

public class W2PdfParseResult
{
    public List<W2ImportRecord> Records { get; set; } = new();

    public List<W2ImportError> Errors { get; set; } = new();
}
