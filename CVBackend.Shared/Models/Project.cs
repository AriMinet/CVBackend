using CVBackend.Shared.Models.Base;

namespace CVBackend.Shared.Models;

/// <summary>
/// Represents a project completed during work experience.
/// Inherits from BaseEntity to follow DRY principle.
/// </summary>
public class Project : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the project.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the foreign key to the company where this project was completed.
    /// </summary>
    public Guid? CompanyId { get; set; }

    /// <summary>
    /// Gets or sets the description of the project and contributions.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the technologies used in the project (comma-separated or JSON).
    /// </summary>
    public string? Technologies { get; set; }

    /// <summary>
    /// Gets or sets the start date of the project.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of the project. Null if ongoing.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the company where this project was completed.
    /// </summary>
    public Company? Company { get; set; }

    /// <summary>
    /// Gets or sets the collection of skills used in this project.
    /// Many-to-many relationship with Skills.
    /// </summary>
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
}
