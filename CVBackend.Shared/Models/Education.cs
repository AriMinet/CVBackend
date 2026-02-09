using CVBackend.Shared.Models.Base;
using CVBackend.Shared.Models.Enums;

namespace CVBackend.Shared.Models;

/// <summary>
/// Represents an educational qualification or degree.
/// Inherits from BaseEntity to follow DRY principle.
/// </summary>
public class Education : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the educational institution.
    /// </summary>
    public string Institution { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the degree or qualification type.
    /// </summary>
    public Enum_DegreeType Degree { get; set; }

    /// <summary>
    /// Gets or sets the field of study or major.
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the start date of the education.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the education. Null if currently studying.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets additional description, achievements, or coursework.
    /// </summary>
    public string? Description { get; set; }
}
