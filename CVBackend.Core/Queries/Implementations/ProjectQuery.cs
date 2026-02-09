using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Models;
using CVBackend.Shared.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CVBackend.Core.Queries.Implementations;

/// <summary>
/// Implementation of project-related queries.
/// </summary>
public class ProjectQuery : IProjectQuery
{
    private readonly CvDbContext _context;
    private readonly ILogger<ProjectQuery> _logger;

    /// <summary>
    /// Initializes a new instance of the ProjectQuery class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public ProjectQuery(CvDbContext context, ILogger<ProjectQuery> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<Project>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all projects");
        List<Project> projects = await _context.Projects
            .OrderBy(p => p.Name)
            .ToListAsync();
        _logger.LogInformation("Retrieved {Count} projects", projects.Count);
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
        _logger.LogInformation("Fetching all projects with relations");
        List<Project> projects = await _context.Projects
            .Include(p => p.Company)
            .Include(p => p.Skills)
            .OrderBy(p => p.Name)
            .ToListAsync();
        _logger.LogInformation("Retrieved {Count} projects with relations", projects.Count);
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
