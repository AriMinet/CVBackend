using CVBackend.Shared.Models.Base;
using CVBackend.Shared.Models.Enums;

namespace CVBackend.Shared.Models;

/// <summary>
/// Represents a technical or professional skill.
/// Inherits from BaseEntity to follow DRY principle.
/// </summary>
public class Skill : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the skill.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category of the skill (e.g., Backend, Frontend, Database).
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the proficiency level.
    /// </summary>
    public Enum_ProficiencyLevel ProficiencyLevel { get; set; }

    /// <summary>
    /// Gets or sets the number of years of experience with this skill.
    /// </summary>
    public int YearsExperience { get; set; }

    /// <summary>
    /// Gets or sets the collection of projects where this skill was used.
    /// Many-to-many relationship with Projects.
    /// </summary>
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
