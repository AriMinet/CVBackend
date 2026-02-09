using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Models;
using CVBackend.Shared.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CVBackend.Core.Queries.Implementations;

/// <summary>
/// Implementation of company-related queries.
/// </summary>
public class CompanyQuery : BaseQuery, ICompanyQuery
{
    /// <summary>
    /// Initializes a new instance of the CompanyQuery class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="configuration">The configuration instance.</param>
    public CompanyQuery(CvDbContext context, ILogger<CompanyQuery> logger, IMemoryCache cache, IConfiguration configuration)
        : base(context, logger, cache, configuration)
    {
    }

    /// <inheritdoc />
    public async Task<List<Company>> GetAllAsync()
    {
        string cacheKey = "companies_all";

        if (_cachingEnabled && _cache.TryGetValue(cacheKey, out List<Company>? cachedCompanies))
        {
            _logger.LogInformation("Cache hit - returning {Count} cached companies", cachedCompanies!.Count);
            return cachedCompanies;
        }

        _logger.LogInformation("Cache miss - fetching all companies from database");
        List<Company> companies = await _context.Companies
            .OrderBy(c => c.Name)
            .ToListAsync();

        if (_cachingEnabled)
        {
            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheExpirationMinutes)
            };
            _cache.Set(cacheKey, companies, cacheOptions);
            _logger.LogInformation("Cached {Count} companies for {Minutes} minutes", companies.Count, _cacheExpirationMinutes);
            return companies;
        }
        
        _logger.LogInformation("Retrieved {Count} companies (caching disabled)", companies.Count);

        return companies;
    }

    /// <inheritdoc />
    public async Task<Company?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching company with ID: {CompanyId}", id);
        Company? company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == id);
        if (company == null)
            _logger.LogWarning("Company not found: {CompanyId}", id);
        return company;
    }

    /// <inheritdoc />
    public async Task<List<Company>> GetAllWithProjectsAsync()
    {
        string cacheKey = "companies_all_with_projects";

        if (_cachingEnabled && _cache.TryGetValue(cacheKey, out List<Company>? cachedCompanies))
        {
            _logger.LogInformation("Cache hit - returning {Count} cached companies with projects", cachedCompanies!.Count);
            return cachedCompanies;
        }

        _logger.LogInformation("Cache miss - fetching all companies with projects from database");
        List<Company> companies = await _context.Companies
            .Include(c => c.Projects)
            .OrderBy(c => c.Name)
            .ToListAsync();

        foreach (Company company in companies)
            company.Projects = company.Projects.OrderBy(p => p.Name).ToList();

        if (_cachingEnabled)
        {
            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheExpirationMinutes)
            };
            _cache.Set(cacheKey, companies, cacheOptions);
            _logger.LogInformation("Cached {Count} companies with projects for {Minutes} minutes", companies.Count, _cacheExpirationMinutes);
            return companies;
        }
        
        _logger.LogInformation("Retrieved {Count} companies with projects (caching disabled)", companies.Count);

        return companies;
    }

    /// <inheritdoc />
    public async Task<Company?> GetByIdWithProjectsAsync(Guid id)
    {
        _logger.LogInformation("Fetching company with projects, ID: {CompanyId}", id);
        Company? company = await _context.Companies
            .Include(c => c.Projects)
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (company == null)
            _logger.LogWarning("Company with projects not found: {CompanyId}", id);
        
        return company;
    }
}
