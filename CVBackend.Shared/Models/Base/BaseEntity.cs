namespace CVBackend.Shared.Models.Base;

/// <summary>
/// Base class for all entities.
/// Follows Don't Repeat Yourself (DRY) principle - centralized common properties.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// Initialized to a new GUID when entity is created.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntity"/> class.
    /// Generates a new GUID for the entity identifier.
    /// </summary>
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
    }
}
