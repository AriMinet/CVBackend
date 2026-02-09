using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Models;
using CVBackend.Shared.Queries.Interfaces;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;

namespace CVBackend.Core.GraphQL.Resolvers;

/// <summary>
/// GraphQL resolver for company-related queries.
/// </summary>
[ExtendObjectType("Query")]
public class CompanyResolver
{
    /// <summary>
    /// Retrieves all companies (non-paginated for backwards compatibility).
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <returns>Queryable of all companies.</returns>
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Company> GetCompanies([Service] CvDbContext context)
    {
        return context.Companies.OrderBy(c => c.Name);
    }

    /// <summary>
    /// Retrieves all companies with cursor-based pagination.
    /// Use this for large datasets. Returns Connection type with edges/nodes.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <returns>Paginated connection of companies.</returns>
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Company> GetCompaniesPaged([Service] CvDbContext context)
    {
        return context.Companies.OrderBy(c => c.Name);
    }

    /// <summary>
    /// Retrieves a company by its unique identifier.
    /// </summary>
    /// <param name="id">The company identifier.</param>
    /// <param name="companyQuery">The company query service.</param>
    /// <returns>The company if found, otherwise null.</returns>
    public async Task<Company?> GetCompanyAsync(Guid id, [Service] ICompanyQuery companyQuery)
    {
        return await companyQuery.GetByIdAsync(id);
    }

    /// <summary>
    /// Retrieves all companies with their associated projects.
    /// </summary>
    /// <param name="companyQuery">The company query service.</param>
    /// <returns>List of companies with projects.</returns>
    public async Task<List<Company>> GetCompaniesWithProjectsAsync([Service] ICompanyQuery companyQuery)
    {
        return await companyQuery.GetAllWithProjectsAsync();
    }
}
