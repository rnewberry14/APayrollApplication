using ClearPathPayroll.Data;
using ClearPathPayroll.Domain;
using Microsoft.EntityFrameworkCore;
using UglyToad.PdfPig;

namespace ClearPathPayroll.Services;

public class W2PdfImportService
{
    private const string PrototypeUserId = "local-prototype-user";
    private readonly PayrollDbContext _context;
    private readonly W2PdfTextParser _parser;

    public W2PdfImportService(PayrollDbContext context, W2PdfTextParser parser)
    {
        _context = context;
        _parser = parser;
    }

    public async Task<W2ImportBatch> ParsePdfAsync(Stream pdfStream, string fileName)
    {
        var extractedText = await ExtractTextFromPdfAsync(pdfStream);
        var parseResult = await ParseW2TextAsync(extractedText);

        var batch = new W2ImportBatch
        {
            FileName = Path.GetFileName(fileName),
            ImportedAt = DateTime.UtcNow,
            ImportedByUserId = PrototypeUserId,
            ImportStatus = parseResult.Records.Count > 0 && parseResult.Errors.All(error => error.Severity != W2ImportErrorSeverity.Error)
                ? W2ImportStatus.NeedsReview
                : W2ImportStatus.Failed,
            RecordCount = parseResult.Records.Count,
            ErrorCount = parseResult.Errors.Count,
            Notes = string.IsNullOrWhiteSpace(extractedText)
                ? "No selectable text was extracted. Use manual entry or spreadsheet import."
                : "Parsed locally from selected PDF. Original PDF was not stored.",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Records = parseResult.Records,
            Errors = parseResult.Errors
        };

        foreach (var record in batch.Records)
        {
            record.W2ImportBatch = batch;
        }

        foreach (var error in batch.Errors)
        {
            error.W2ImportBatch = batch;
        }

        return batch;
    }

    public Task<string> ExtractTextFromPdfAsync(Stream pdfStream)
    {
        using var document = PdfDocument.Open(pdfStream);
        var pages = document.GetPages().Select(page => page.Text);
        return Task.FromResult(string.Join(Environment.NewLine, pages));
    }

    public Task<W2PdfParseResult> ParseW2TextAsync(string extractedText)
    {
        return Task.FromResult(_parser.Parse(extractedText));
    }

    public List<W2ImportError> ValidateRecord(W2ImportRecord record)
    {
        var errors = new List<W2ImportError>();

        if (record.TaxYear == null)
        {
            errors.Add(CreateError("TaxYear", "TaxYear is required."));
        }
        else if (record.TaxYear < 1900 || record.TaxYear > DateTime.Today.Year + 1)
        {
            errors.Add(CreateError("TaxYear", "TaxYear is outside the accepted range."));
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
            errors.Add(CreateWarning("EmployeeSSNLast4Only", "Employee SSN last 4 is missing. Review before saving."));
        }
        else
        {
            record.EmployeeSSNLast4Only = W2PdfTextParser.Last4(record.EmployeeSSNLast4Only);
            if (record.EmployeeSSNLast4Only.Length != 4)
            {
                errors.Add(CreateWarning("EmployeeSSNLast4Only", "Employee SSN last 4 should contain four digits."));
            }
        }

        record.EmployerEINLast4Only = W2PdfTextParser.Last4(record.EmployerEINLast4Only);

        var amounts = GetAmountFields(record).ToList();
        if (!amounts.Any(item => item.Value.HasValue))
        {
            errors.Add(CreateError("Amounts", "At least one wage or tax amount is required."));
        }

        foreach (var amount in amounts.Where(item => item.Value < 0))
        {
            errors.Add(CreateWarning(amount.FieldName, "Negative amount is flagged for review."));
        }

        record.RawExtractedTextPreview = W2PdfTextParser.BuildSafePreview(record.RawExtractedTextPreview);
        record.NeedsManualReview = record.NeedsManualReview || errors.Any(error => error.Severity != W2ImportErrorSeverity.Info);
        record.UpdatedAt = DateTime.UtcNow;

        return errors;
    }

    public async Task<W2ImportBatch> SaveReviewedImportAsync(W2ImportBatch batch, IEnumerable<W2ImportRecord> records)
    {
        var recordList = records.ToList();
        if (recordList.Count == 0)
        {
            throw new InvalidOperationException("At least one reviewed W-2 record is required.");
        }

        if (recordList.Any(record => !record.UserConfirmed))
        {
            throw new InvalidOperationException("Reviewed W-2 records require user confirmation before saving.");
        }

        var validationErrors = recordList.SelectMany(ValidateRecord).ToList();
        if (validationErrors.Any(error => error.Severity == W2ImportErrorSeverity.Error))
        {
            throw new InvalidOperationException("W-2 PDF import has validation errors that block saving.");
        }

        batch.ImportStatus = W2ImportStatus.Confirmed;
        batch.RecordCount = recordList.Count;
        batch.ErrorCount = validationErrors.Count;
        batch.UpdatedAt = DateTime.UtcNow;
        batch.Records = recordList;
        batch.Errors = validationErrors;

        _context.W2ImportBatches.Add(batch);
        await _context.SaveChangesAsync();
        return await _context.W2ImportBatches
            .Include(saved => saved.Records)
            .Include(saved => saved.Errors)
            .FirstAsync(saved => saved.W2ImportBatchId == batch.W2ImportBatchId);
    }

    private static IEnumerable<(string FieldName, decimal? Value)> GetAmountFields(W2ImportRecord record)
    {
        yield return (nameof(record.Box1WagesTipsOtherCompensation), record.Box1WagesTipsOtherCompensation);
        yield return (nameof(record.Box2FederalIncomeTaxWithheld), record.Box2FederalIncomeTaxWithheld);
        yield return (nameof(record.Box3SocialSecurityWages), record.Box3SocialSecurityWages);
        yield return (nameof(record.Box4SocialSecurityTaxWithheld), record.Box4SocialSecurityTaxWithheld);
        yield return (nameof(record.Box5MedicareWagesAndTips), record.Box5MedicareWagesAndTips);
        yield return (nameof(record.Box6MedicareTaxWithheld), record.Box6MedicareTaxWithheld);
        yield return (nameof(record.StateWages1), record.StateWages1);
        yield return (nameof(record.StateIncomeTax1), record.StateIncomeTax1);
        yield return (nameof(record.LocalWages1), record.LocalWages1);
        yield return (nameof(record.LocalIncomeTax1), record.LocalIncomeTax1);
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
