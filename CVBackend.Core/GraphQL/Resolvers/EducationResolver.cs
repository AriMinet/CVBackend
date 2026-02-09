using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;
using CVBackend.Shared.Queries.Interfaces;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;

namespace CVBackend.Core.GraphQL.Resolvers;

/// <summary>
/// GraphQL resolver for education-related queries.
/// </summary>
[ExtendObjectType("Query")]
public class EducationResolver
{
    /// <summary>
    /// Retrieves all education entries (non-paginated for backwards compatibility).
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <returns>Queryable of all education entries.</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Education> GetAllEducation([Service] CvDbContext context)
    {
        return context.Education.OrderBy(e => e.Institution);
    }

    /// <summary>
    /// Retrieves all education entries with cursor-based pagination.
    /// Use this for large datasets. Returns Connection type with edges/nodes.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <returns>Paginated connection of education entries.</returns>
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Education> GetAllEducationPaged([Service] CvDbContext context)
    {
        return context.Education.OrderBy(e => e.Institution);
    }

    /// <summary>
    /// Retrieves an education entry by its unique identifier.
    /// </summary>
    /// <param name="id">The education identifier.</param>
    /// <param name="educationQuery">The education query service.</param>
    /// <returns>The education entry if found, otherwise null.</returns>
    public async Task<Education?> GetEducationAsync(Guid id, [Service] IEducationQuery educationQuery)
    {
        return await educationQuery.GetByIdAsync(id);
    }

    /// <summary>
    /// Retrieves education entries filtered by degree type.
    /// </summary>
    /// <param name="degree">The degree type to filter by.</param>
    /// <param name="educationQuery">The education query service.</param>
    /// <returns>List of education entries with the specified degree type.</returns>
    public async Task<List<Education>> GetEducationByDegreeAsync(
        Enum_DegreeType degree,
        [Service] IEducationQuery educationQuery)
    {
        return await educationQuery.GetByDegreeTypeAsync(degree);
    }

    /// <summary>
    /// Retrieves education entries filtered by institution name.
    /// </summary>
    /// <param name="institution">The institution name to search for.</param>
    /// <param name="educationQuery">The education query service.</param>
    /// <returns>List of education entries from the specified institution.</returns>
    public async Task<List<Education>> GetEducationByInstitutionAsync(
        string institution,
        [Service] IEducationQuery educationQuery)
    {
        return await educationQuery.GetByInstitutionAsync(institution);
    }
}
