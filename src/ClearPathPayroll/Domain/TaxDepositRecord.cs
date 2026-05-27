using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

[Table("TaxDepositRecords")]
public class TaxDepositRecord
{
    public int TaxDepositRecordId { get; set; }

    [Required]
    public int CompanyId { get; set; }

    public Company? Company { get; set; }

    [Required]
    public DateTime DepositDate { get; set; }

    [Required]
    public DateTime TaxPeriodStart { get; set; }

    [Required]
    public DateTime TaxPeriodEnd { get; set; }

    [Required]
    [StringLength(100)]
    public string TaxType { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Agency { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [StringLength(100)]
    public string? ConfirmationNumber { get; set; }

    [Required]
    [StringLength(100)]
    public string PaymentMethod { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Notes { get; set; }

    [Required]
    [StringLength(100)]
    public string RecordSource { get; set; } = "User-entered deposit record";

    public int? ImportBatchId { get; set; }

    public ImportBatch? ImportBatch { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
