using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;

namespace CVBackend.Shared.Queries.Interfaces;

/// <summary>
/// Interface for education-related queries.
/// </summary>
public interface IEducationQuery
{
    /// <summary>
    /// Retrieves all education entries.
    /// </summary>
    /// <returns>List of all education entries.</returns>
    Task<List<Education>> GetAllAsync();

    /// <summary>
    /// Retrieves an education entry by its unique identifier.
    /// </summary>
    /// <param name="id">The education identifier.</param>
    /// <returns>The education entry if found, otherwise null.</returns>
    Task<Education?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves education entries filtered by degree type.
    /// </summary>
    /// <param name="degreeType">The degree type to filter by.</param>
    /// <returns>List of education entries with the specified degree type.</returns>
    Task<List<Education>> GetByDegreeTypeAsync(Enum_DegreeType degreeType);

    /// <summary>
    /// Retrieves education entries filtered by institution name.
    /// </summary>
    /// <param name="institution">The institution name to search for.</param>
    /// <returns>List of education entries from the specified institution.</returns>
    Task<List<Education>> GetByInstitutionAsync(string institution);
}
