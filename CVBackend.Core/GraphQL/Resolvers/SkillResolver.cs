using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;
using CVBackend.Shared.Queries.Interfaces;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;

namespace CVBackend.Core.GraphQL.Resolvers;

/// <summary>
/// GraphQL resolver for skill-related queries.
/// </summary>
[ExtendObjectType("Query")]
public class SkillResolver
{
    /// <summary>
    /// Retrieves all skills (non-paginated for backwards compatibility).
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <returns>Queryable of all skills.</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Skill> GetSkills([Service] CvDbContext context)
    {
        return context.Skills.OrderBy(s => s.Name);
    }

    /// <summary>
    /// Retrieves all skills with cursor-based pagination.
    /// Use this for large datasets. Returns Connection type with edges/nodes.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <returns>Paginated connection of skills.</returns>
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Skill> GetSkillsPaged([Service] CvDbContext context)
    {
        return context.Skills.OrderBy(s => s.Name);
    }

    /// <summary>
    /// Retrieves a skill by its unique identifier.
    /// </summary>
    /// <param name="id">The skill identifier.</param>
    /// <param name="skillQuery">The skill query service.</param>
    /// <returns>The skill if found, otherwise null.</returns>
    public async Task<Skill?> GetSkillAsync(Guid id, [Service] ISkillQuery skillQuery)
    {
        return await skillQuery.GetByIdAsync(id);
    }

    /// <summary>
    /// Retrieves skills filtered by category.
    /// </summary>
    /// <param name="category">The category to filter by.</param>
    /// <param name="skillQuery">The skill query service.</param>
    /// <returns>List of skills in the specified category.</returns>
    public async Task<List<Skill>> GetSkillsByCategoryAsync(
        string category,
        [Service] ISkillQuery skillQuery)
    {
        return await skillQuery.GetByCategoryAsync(category);
    }

    /// <summary>
    /// Retrieves skills filtered by proficiency level.
    /// </summary>
    /// <param name="proficiencyLevel">The proficiency level to filter by.</param>
    /// <param name="skillQuery">The skill query service.</param>
    /// <returns>List of skills with the specified proficiency level.</returns>
    public async Task<List<Skill>> GetSkillsByProficiencyAsync(
        Enum_ProficiencyLevel proficiencyLevel,
        [Service] ISkillQuery skillQuery)
    {
        return await skillQuery.GetByProficiencyLevelAsync(proficiencyLevel);
    }

    /// <summary>
    /// Retrieves all skills with their associated projects.
    /// </summary>
    /// <param name="skillQuery">The skill query service.</param>
    /// <returns>List of skills with projects.</returns>
    public async Task<List<Skill>> GetSkillsWithProjectsAsync([Service] ISkillQuery skillQuery)
    {
        return await skillQuery.GetAllWithProjectsAsync();
    }

    /// <summary>
    /// Retrieves skills used in a specific project.
    /// </summary>
    /// <param name="projectId">The project identifier.</param>
    /// <param name="skillQuery">The skill query service.</param>
    /// <returns>List of skills used in the specified project.</returns>
    public async Task<List<Skill>> GetSkillsByProjectAsync(
        Guid projectId,
        [Service] ISkillQuery skillQuery)
    {
        return await skillQuery.GetByProjectIdAsync(projectId);
    }
}
