using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

/// <summary>
/// Pay frequency enumeration.
/// </summary>
public enum PayFrequency
{
    Weekly,
    Biweekly,
    Semimonthly,
    Monthly
}

/// <summary>
/// Represents a pay schedule for a company.
/// </summary>
[Table("PaySchedules")]
public class PaySchedule
{
    /// <summary>
    /// Unique identifier for the pay schedule.
    /// </summary>
    [Key]
    public int PayScheduleId { get; set; }

    /// <summary>
    /// Foreign key to the company.
    /// </summary>
    [Required]
    public int CompanyId { get; set; }

    /// <summary>
    /// Navigation property to the company.
    /// </summary>
    [ForeignKey("CompanyId")]
    public Company? Company { get; set; }

    /// <summary>
    /// Name of the pay schedule.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Frequency of the pay schedule.
    /// </summary>
    [Required]
    public PayFrequency Frequency { get; set; }

    /// <summary>
    /// Next pay date.
    /// </summary>
    [Required]
    public DateTime NextPayDate { get; set; }

    /// <summary>
    /// Start date of the next pay period.
    /// </summary>
    [Required]
    public DateTime NextPeriodStartDate { get; set; }

    /// <summary>
    /// End date of the next pay period.
    /// </summary>
    [Required]
    public DateTime NextPeriodEndDate { get; set; }

    /// <summary>
    /// Indicates if the pay schedule is active.
    /// </summary>
    [Required]
    public bool IsActive { get; set; } = true;
}