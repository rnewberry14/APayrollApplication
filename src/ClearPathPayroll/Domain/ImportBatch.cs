using System.ComponentModel.DataAnnotations;

namespace ClearPathPayroll.Domain;

public class ImportBatch
{
    public int ImportBatchId { get; set; }

    public ImportType ImportType { get; set; }

    public ImportFileFormat FileFormat { get; set; }

    public ImportBatchStatus Status { get; set; } = ImportBatchStatus.Parsed;

    [Required]
    [StringLength(260)]
    public string FileName { get; set; } = string.Empty;

    public int RowCount { get; set; }

    public int ValidRowCount { get; set; }

    public int ErrorCount { get; set; }

    public bool DemoModeWarningAcknowledged { get; set; }

    [StringLength(100)]
    public string? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? ConfirmedAt { get; set; }

    public ICollection<ImportRow> Rows { get; set; } = new List<ImportRow>();

    public ICollection<ImportError> Errors { get; set; } = new List<ImportError>();

    public ICollection<ImportMapping> Mappings { get; set; } = new List<ImportMapping>();
}

public enum ImportType
{
    EntireCompany = 0,
    EmployersOnly = 1,
    EmployeesOnly = 2,
    EmployeesAndChecksOnly = 3,
    ChecksOnly = 4,
    TaxDepositsOnly = 5,
    W2Historical = 6
}

public enum ImportFileFormat
{
    Csv = 0,
    TabDelimited = 1,
    Excel = 2
}

public enum ImportBatchStatus
{
    Parsed = 0,
    Mapped = 1,
    ValidationFailed = 2,
    ReadyForConfirmation = 3,
    Imported = 4,
    Failed = 5
}
