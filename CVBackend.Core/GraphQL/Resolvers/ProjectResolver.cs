using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Models;
using CVBackend.Shared.Queries.Interfaces;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;

namespace CVBackend.Core.GraphQL.Resolvers;

/// <summary>
/// GraphQL resolver for project-related queries.
/// </summary>
[ExtendObjectType("Query")]
public class ProjectResolver
{
    /// <summary>
    /// Retrieves all projects (non-paginated for backwards compatibility).
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <returns>Queryable of all projects.</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Project> GetProjects([Service] CvDbContext context)
    {
        return context.Projects.OrderBy(p => p.Name);
    }

    /// <summary>
    /// Retrieves all projects with cursor-based pagination.
    /// Use this for large datasets. Returns Connection type with edges/nodes.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <returns>Paginated connection of projects.</returns>
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Project> GetProjectsPaged([Service] CvDbContext context)
    {
        return context.Projects.OrderBy(p => p.Name);
    }

    /// <summary>
    /// Retrieves a project by its unique identifier.
    /// </summary>
    /// <param name="id">The project identifier.</param>
    /// <param name="projectQuery">The project query service.</param>
    /// <returns>The project if found, otherwise null.</returns>
    public async Task<Project?> GetProjectAsync(Guid id, [Service] IProjectQuery projectQuery)
    {
        return await projectQuery.GetByIdAsync(id);
    }

    /// <summary>
    /// Retrieves all projects with their related entities (company and skills).
    /// </summary>
    /// <param name="projectQuery">The project query service.</param>
    /// <returns>List of projects with relations.</returns>
    public async Task<List<Project>> GetProjectsWithRelationsAsync([Service] IProjectQuery projectQuery)
    {
        return await projectQuery.GetAllWithRelationsAsync();
    }

    /// <summary>
    /// Retrieves projects by company identifier.
    /// </summary>
    /// <param name="companyId">The company identifier.</param>
    /// <param name="projectQuery">The project query service.</param>
    /// <returns>List of projects for the specified company.</returns>
    public async Task<List<Project>> GetProjectsByCompanyAsync(Guid companyId, [Service] IProjectQuery projectQuery)
    {
        return await projectQuery.GetByCompanyIdAsync(companyId);
    }

    /// <summary>
    /// Retrieves projects that use a specific skill.
    /// </summary>
    /// <param name="skillId">The skill identifier.</param>
    /// <param name="projectQuery">The project query service.</param>
    /// <returns>List of projects that use the specified skill.</returns>
    public async Task<List<Project>> GetProjectsBySkillAsync(
        Guid skillId,
        [Service] IProjectQuery projectQuery)
    {
        return await projectQuery.GetBySkillIdAsync(skillId);
    }
}
