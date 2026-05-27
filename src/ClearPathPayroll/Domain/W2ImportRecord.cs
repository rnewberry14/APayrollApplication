using System.ComponentModel.DataAnnotations;

namespace ClearPathPayroll.Domain;

public class W2ImportRecord
{
    public int W2ImportRecordId { get; set; }

    public int W2ImportBatchId { get; set; }

    public W2ImportBatch? W2ImportBatch { get; set; }

    public int? TaxYear { get; set; }

    [StringLength(150)]
    public string EmployerName { get; set; } = string.Empty;

    [StringLength(4)]
    public string EmployerEINLast4Only { get; set; } = string.Empty;

    [StringLength(300)]
    public string EmployerAddress { get; set; } = string.Empty;

    [StringLength(80)]
    public string EmployeeFirstName { get; set; } = string.Empty;

    [StringLength(1)]
    public string? EmployeeMiddleInitial { get; set; }

    [StringLength(80)]
    public string EmployeeLastName { get; set; } = string.Empty;

    [StringLength(4)]
    public string EmployeeSSNLast4Only { get; set; } = string.Empty;

    [StringLength(300)]
    public string EmployeeAddress { get; set; } = string.Empty;

    public decimal? Box1WagesTipsOtherCompensation { get; set; }
    public decimal? Box2FederalIncomeTaxWithheld { get; set; }
    public decimal? Box3SocialSecurityWages { get; set; }
    public decimal? Box4SocialSecurityTaxWithheld { get; set; }
    public decimal? Box5MedicareWagesAndTips { get; set; }
    public decimal? Box6MedicareTaxWithheld { get; set; }
    public decimal? Box7SocialSecurityTips { get; set; }
    public decimal? Box8AllocatedTips { get; set; }
    public decimal? Box10DependentCareBenefits { get; set; }
    public decimal? Box11NonqualifiedPlans { get; set; }

    [StringLength(4)]
    public string? Box12CodeA { get; set; }
    public decimal? Box12AmountA { get; set; }

    [StringLength(4)]
    public string? Box12CodeB { get; set; }
    public decimal? Box12AmountB { get; set; }

    [StringLength(4)]
    public string? Box12CodeC { get; set; }
    public decimal? Box12AmountC { get; set; }

    [StringLength(4)]
    public string? Box12CodeD { get; set; }
    public decimal? Box12AmountD { get; set; }

    public bool Box13StatutoryEmployee { get; set; }
    public bool Box13RetirementPlan { get; set; }
    public bool Box13ThirdPartySickPay { get; set; }

    [StringLength(100)]
    public string? Box14OtherDescription1 { get; set; }
    public decimal? Box14OtherAmount1 { get; set; }

    [StringLength(100)]
    public string? Box14OtherDescription2 { get; set; }
    public decimal? Box14OtherAmount2 { get; set; }

    [StringLength(100)]
    public string? Box14OtherDescription3 { get; set; }
    public decimal? Box14OtherAmount3 { get; set; }

    [StringLength(2)]
    public string? State1 { get; set; }

    [StringLength(30)]
    public string? EmployerStateId1Masked { get; set; }

    public decimal? StateWages1 { get; set; }
    public decimal? StateIncomeTax1 { get; set; }

    [StringLength(100)]
    public string? LocalityName1 { get; set; }

    public decimal? LocalWages1 { get; set; }
    public decimal? LocalIncomeTax1 { get; set; }

    [StringLength(2)]
    public string? State2 { get; set; }

    [StringLength(30)]
    public string? EmployerStateId2Masked { get; set; }

    public decimal? StateWages2 { get; set; }
    public decimal? StateIncomeTax2 { get; set; }

    [StringLength(100)]
    public string? LocalityName2 { get; set; }

    public decimal? LocalWages2 { get; set; }
    public decimal? LocalIncomeTax2 { get; set; }

    [StringLength(2000)]
    public string RawExtractedTextPreview { get; set; } = string.Empty;

    public decimal ConfidenceScore { get; set; }

    public bool NeedsManualReview { get; set; } = true;

    public bool UserConfirmed { get; set; }

    [StringLength(1000)]
    public string? ReviewNotes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
