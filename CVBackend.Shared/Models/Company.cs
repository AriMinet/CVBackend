using CVBackend.Shared.Models.Base;

namespace CVBackend.Shared.Models;

/// <summary>
/// Represents a company where work experience was gained.
/// Inherits from BaseEntity to follow DRY principle.
/// </summary>
public class Company : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the company.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the job position held at the company.
    /// </summary>
    public string Position { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the start date of employment.
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date of employment. Null if currently employed.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Gets or sets the description of responsibilities and achievements.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the collection of projects completed at this company.
    /// </summary>
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}
