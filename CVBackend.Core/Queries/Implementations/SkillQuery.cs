using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;
using CVBackend.Shared.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CVBackend.Core.Queries.Implementations;

/// <summary>
/// Implementation of skill-related queries.
/// </summary>
public class SkillQuery : ISkillQuery
{
    private readonly CvDbContext _context;
    private readonly ILogger<SkillQuery> _logger;

    /// <summary>
    /// Initializes a new instance of the SkillQuery class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public SkillQuery(CvDbContext context, ILogger<SkillQuery> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<Skill>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all skills");
        List<Skill> skills = await _context.Skills
            .OrderBy(s => s.Category)
            .ThenBy(s => s.Name)
            .ToListAsync();
        _logger.LogInformation("Retrieved {Count} skills", skills.Count);
        return skills;
    }

    /// <inheritdoc />
    public async Task<Skill?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching skill with ID: {SkillId}", id);
        Skill? skill = await _context.Skills
            .FirstOrDefaultAsync(s => s.Id == id);
        if (skill == null)
            _logger.LogWarning("Skill not found: {SkillId}", id);
        return skill;
    }

    /// <inheritdoc />
    public async Task<List<Skill>> GetByCategoryAsync(string category)
    {
        _logger.LogInformation("Fetching skills by category: {Category}", category);
        List<Skill> skills = await _context.Skills
            .Where(s => s.Category == category)
            .OrderBy(s => s.Name)
            .ToListAsync();
        _logger.LogInformation("Retrieved {Count} skills in category: {Category}", skills.Count, category);
        return skills;
    }

    /// <inheritdoc />
    public async Task<List<Skill>> GetByProficiencyLevelAsync(Enum_ProficiencyLevel proficiencyLevel)
    {
        _logger.LogInformation("Fetching skills by proficiency level: {ProficiencyLevel}", proficiencyLevel);
        List<Skill> skills = await _context.Skills
            .Where(s => s.ProficiencyLevel == proficiencyLevel)
            .OrderBy(s => s.Name)
            .ToListAsync();
        _logger.LogInformation("Retrieved {Count} skills with proficiency: {ProficiencyLevel}", skills.Count, proficiencyLevel);
        return skills;
    }

    /// <inheritdoc />
    public async Task<List<Skill>> GetAllWithProjectsAsync()
    {
        _logger.LogInformation("Fetching all skills with projects");
        List<Skill> skills = await _context.Skills
            .Include(s => s.Projects)
            .OrderBy(s => s.Category)
            .ThenBy(s => s.Name)
            .ToListAsync();
        _logger.LogInformation("Retrieved {Count} skills with projects", skills.Count);
        return skills;
    }

    /// <inheritdoc />
    public async Task<List<Skill>> GetByProjectIdAsync(Guid projectId)
    {
        _logger.LogInformation("Fetching skills for project: {ProjectId}", projectId);
        List<Skill> skills = await _context.Skills
            .Where(s => s.Projects.Any(p => p.Id == projectId))
            .OrderBy(s => s.Category)
            .ThenBy(s => s.Name)
            .ToListAsync();
        _logger.LogInformation("Retrieved {Count} skills for project: {ProjectId}", skills.Count, projectId);
        return skills;
    }
}
