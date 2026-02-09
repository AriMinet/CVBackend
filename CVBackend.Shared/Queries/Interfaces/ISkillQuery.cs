using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;

namespace CVBackend.Shared.Queries.Interfaces;

/// <summary>
/// Interface for skill-related queries.
/// </summary>
public interface ISkillQuery
{
    /// <summary>
    /// Retrieves all skills.
    /// </summary>
    /// <returns>List of all skills.</returns>
    Task<List<Skill>> GetAllAsync();

    /// <summary>
    /// Retrieves a skill by its unique identifier.
    /// </summary>
    /// <param name="id">The skill identifier.</param>
    /// <returns>The skill if found, otherwise null.</returns>
    Task<Skill?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves skills filtered by category.
    /// </summary>
    /// <param name="category">The category to filter by.</param>
    /// <returns>List of skills in the specified category.</returns>
    Task<List<Skill>> GetByCategoryAsync(string category);

    /// <summary>
    /// Retrieves skills filtered by proficiency level.
    /// </summary>
    /// <param name="proficiencyLevel">The proficiency level to filter by.</param>
    /// <returns>List of skills with the specified proficiency level.</returns>
    Task<List<Skill>> GetByProficiencyLevelAsync(Enum_ProficiencyLevel proficiencyLevel);

    /// <summary>
    /// Retrieves all skills with their associated projects loaded.
    /// </summary>
    /// <returns>List of skills with projects.</returns>
    Task<List<Skill>> GetAllWithProjectsAsync();

    /// <summary>
    /// Retrieves skills used in a specific project.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <returns>List of skills used in the specified project.</returns>
    Task<List<Skill>> GetByProjectIdAsync(Guid projectId);
}
