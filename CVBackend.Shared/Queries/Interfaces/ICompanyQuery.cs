using CVBackend.Shared.Models;

namespace CVBackend.Shared.Queries.Interfaces;

/// <summary>
/// Interface for company-related queries.
/// </summary>
public interface ICompanyQuery
{
    /// <summary>
    /// Retrieves all companies.
    /// </summary>
    /// <returns>List of all companies.</returns>
    Task<List<Company>> GetAllAsync();

    /// <summary>
    /// Retrieves a company by its unique identifier.
    /// </summary>
    /// <param name="id">The company identifier.</param>
    /// <returns>The company if found, otherwise null.</returns>
    Task<Company?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all companies with their associated projects loaded.
    /// </summary>
    /// <returns>List of companies with projects.</returns>
    Task<List<Company>> GetAllWithProjectsAsync();

    /// <summary>
    /// Retrieves a company by ID with its associated projects loaded.
    /// </summary>
    /// <param name="id">The company identifier.</param>
    /// <returns>The company with projects if found, otherwise null.</returns>
    Task<Company?> GetByIdWithProjectsAsync(Guid id);
}
