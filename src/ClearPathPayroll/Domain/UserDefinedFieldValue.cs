using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearPathPayroll.Domain;

[Table("UserDefinedFieldValues")]
public class UserDefinedFieldValue : IValidatableObject
{
    [Key]
    public int UserDefinedFieldValueId { get; set; }

    [Required]
    public int FieldDefinitionId { get; set; }

    public UserDefinedFieldDefinition? FieldDefinition { get; set; }

    public UserDefinedFieldAppliesTo EntityType { get; set; }

    [Required]
    public int EntityId { get; set; }

    [StringLength(4000)]
    public string? TextValue { get; set; }

    public int? NumberValue { get; set; }

    public decimal? DecimalValue { get; set; }

    public DateTime? DateValue { get; set; }

    public bool? BooleanValue { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrWhiteSpace(TextValue))
        {
            var lower = TextValue.ToLowerInvariant();
            if (lower.Contains("<script") || lower.Contains("javascript:") || lower.Contains("{{") || lower.Contains("}}"))
            {
                yield return new ValidationResult(
                    "User-defined field values do not allow scripts, templates, or executable expressions.",
                    new[] { nameof(TextValue) });
            }
        }
    }
}
