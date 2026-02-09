using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Models;
using CVBackend.Shared.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CVBackend.Core.Queries.Implementations;

/// <summary>
/// Implementation of project-related queries.
/// </summary>
public class ProjectQuery : BaseQuery, IProjectQuery
{
    /// <summary>
    /// Initializes a new instance of the ProjectQuery class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="configuration">The configuration instance.</param>
    public ProjectQuery(CvDbContext context, ILogger<ProjectQuery> logger, IMemoryCache cache, IConfiguration configuration)
        : base(context, logger, cache, configuration)
    {
    }

    /// <inheritdoc />
    public async Task<List<Project>> GetAllAsync()
    {
        string cacheKey = "projects_all";

        if (_cachingEnabled && _cache.TryGetValue(cacheKey, out List<Project>? cachedProjects))
        {
            _logger.LogInformation("Cache hit - returning {Count} cached projects", cachedProjects!.Count);
            return cachedProjects;
        }

        _logger.LogInformation("Cache miss - fetching all projects from database");
        List<Project> projects = await _context.Projects
            .OrderBy(p => p.Name)
            .ToListAsync();

        if (_cachingEnabled)
        {
            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheExpirationMinutes)
            };
            _cache.Set(cacheKey, projects, cacheOptions);
            _logger.LogInformation("Cached {Count} projects for {Minutes} minutes", projects.Count, _cacheExpirationMinutes);
            return projects;
        }

        _logger.LogInformation("Retrieved {Count} projects (caching disabled)", projects.Count);

        return projects;
    }

    /// <inheritdoc />
    public async Task<Project?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching project with ID: {ProjectId}", id);
        Project? project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id);
        if (project == null)
            _logger.LogWarning("Project not found: {ProjectId}", id);
        return project;
    }

    /// <inheritdoc />
    public async Task<List<Project>> GetAllWithRelationsAsync()
    {
        string cacheKey = "projects_all_with_relations";

        if (_cachingEnabled && _cache.TryGetValue(cacheKey, out List<Project>? cachedProjects))
        {
            _logger.LogInformation("Cache hit - returning {Count} cached projects with relations", cachedProjects!.Count);
            return cachedProjects;
        }

        _logger.LogInformation("Cache miss - fetching all projects with relations from database");
        List<Project> projects = await _context.Projects
            .Include(p => p.Company)
            .Include(p => p.Skills)
            .OrderBy(p => p.Name)
            .ToListAsync();

        if (_cachingEnabled)
        {
            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheExpirationMinutes)
            };
            _cache.Set(cacheKey, projects, cacheOptions);
            _logger.LogInformation("Cached {Count} projects with relations for {Minutes} minutes", projects.Count, _cacheExpirationMinutes);
            return projects;
        }

        _logger.LogInformation("Retrieved {Count} projects with relations (caching disabled)", projects.Count);

        return projects;
    }

    /// <inheritdoc />
    public async Task<List<Project>> GetByCompanyIdAsync(Guid companyId)
    {
        _logger.LogInformation("Fetching projects for company: {CompanyId}", companyId);
        List<Project> projects = await _context.Projects
            .Where(p => p.CompanyId == companyId)
            .OrderBy(p => p.Name)
            .ToListAsync();
        _logger.LogInformation("Retrieved {Count} projects for company: {CompanyId}", projects.Count, companyId);
        return projects;
    }

    /// <inheritdoc />
    public async Task<List<Project>> GetBySkillIdAsync(Guid skillId)
    {
        _logger.LogInformation("Fetching projects using skill: {SkillId}", skillId);
        List<Project> projects = await _context.Projects
            .Where(p => p.Skills.Any(s => s.Id == skillId))
            .OrderBy(p => p.Name)
            .ToListAsync();
        _logger.LogInformation("Retrieved {Count} projects using skill: {SkillId}", projects.Count, skillId);
        return projects;
    }
}
