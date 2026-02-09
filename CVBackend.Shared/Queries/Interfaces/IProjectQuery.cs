using CVBackend.Shared.Models;

namespace CVBackend.Shared.Queries.Interfaces;

/// <summary>
/// Interface for project-related queries.
/// </summary>
public interface IProjectQuery
{
    /// <summary>
    /// Retrieves all projects.
    /// </summary>
    /// <returns>List of all projects.</returns>
    Task<List<Project>> GetAllAsync();

    /// <summary>
    /// Retrieves a project by its unique identifier.
    /// </summary>
    /// <param name="id">The project identifier.</param>
    /// <returns>The project if found, otherwise null.</returns>
    Task<Project?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves all projects with their associated company and skills loaded.
    /// </summary>
    /// <returns>List of projects with related entities.</returns>
    Task<List<Project>> GetAllWithRelationsAsync();

    /// <summary>
    /// Retrieves projects by company ID.
    /// </summary>
    /// <param name="companyId">The company identifier.</param>
    /// <returns>List of projects for the specified company.</returns>
    Task<List<Project>> GetByCompanyIdAsync(Guid companyId);

    /// <summary>
    /// Retrieves projects that use a specific skill.
    /// </summary>
    /// <param name="skillId">The skill identifier.</param>
    /// <returns>List of projects that use the specified skill.</returns>
    Task<List<Project>> GetBySkillIdAsync(Guid skillId);
}
