using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

/// <summary>
/// Represents an audit log entry for payroll operations.
/// </summary>
[Table("AuditLogEntries")]
public class AuditLogEntry
{
    /// <summary>
    /// Unique identifier for the audit log entry.
    /// </summary>
    [Key]
    public int AuditLogEntryId { get; set; }

    /// <summary>
    /// Payroll run associated with the audit event.
    /// </summary>
    public int? PayrollRunId { get; set; }

    /// <summary>
    /// Navigation property to the payroll run.
    /// </summary>
    [ForeignKey("PayrollRunId")]
    public PayrollRun? PayrollRun { get; set; }

    /// <summary>
    /// Event type recorded in the audit log.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable description of the event.
    /// </summary>
    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// User who performed the action.
    /// </summary>
    [StringLength(100)]
    public string? CreatedByUserId { get; set; }

    /// <summary>
    /// Timestamp when the audit event was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}