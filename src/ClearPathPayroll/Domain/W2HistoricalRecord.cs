using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

[Table("W2HistoricalRecords")]
public class W2HistoricalRecord
{
    public int W2HistoricalRecordId { get; set; }

    [Range(1900, 2100)]
    public int TaxYear { get; set; }

    [Required]
    [StringLength(150)]
    public string EmployerName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string EmployerEINMasked { get; set; } = string.Empty;

    [Required]
    [StringLength(300)]
    public string EmployerAddress { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string EmployeeFirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string EmployeeLastName { get; set; } = string.Empty;

    [Required]
    [StringLength(4)]
    [RegularExpression(@"^\d{4}$")]
    public string EmployeeSSNLast4 { get; set; } = string.Empty;

    [Required]
    [StringLength(300)]
    public string EmployeeAddress { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Box1Wages { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Box2FederalTaxWithheld { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Box3SocialSecurityWages { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Box4SocialSecurityTaxWithheld { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Box5MedicareWages { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Box6MedicareTaxWithheld { get; set; }

    [StringLength(1000)]
    public string? Box12CodeAndAmountPlaceholders { get; set; }

    [StringLength(1000)]
    public string? Box14DescriptionAndAmountPlaceholders { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? StateWages { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? StateTaxWithheld { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? LocalWages { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? LocalTaxWithheld { get; set; }

    [Required]
    [StringLength(100)]
    public string RecordSource { get; set; } = "historical user-entered data";

    public int? ImportBatchId { get; set; }

    public ImportBatch? ImportBatch { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
