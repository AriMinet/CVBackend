using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;
using CVBackend.Shared.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CVBackend.Core.Queries.Implementations;

/// <summary>
/// Implementation of skill-related queries.
/// </summary>
public class SkillQuery : ISkillQuery
{
    private readonly CvDbContext _context;

    /// <summary>
    /// Initializes a new instance of the SkillQuery class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public SkillQuery(CvDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<List<Skill>> GetAllAsync()
    {
        return await _context.Skills
            .OrderBy(s => s.Category)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Skill?> GetByIdAsync(Guid id)
    {
        return await _context.Skills
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    /// <inheritdoc />
    public async Task<List<Skill>> GetByCategoryAsync(string category)
    {
        return await _context.Skills
            .Where(s => s.Category == category)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<Skill>> GetByProficiencyLevelAsync(Enum_ProficiencyLevel proficiencyLevel)
    {
        return await _context.Skills
            .Where(s => s.ProficiencyLevel == proficiencyLevel)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<Skill>> GetAllWithProjectsAsync()
    {
        return await _context.Skills
            .Include(s => s.Projects)
            .OrderBy(s => s.Category)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<Skill>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.Skills
            .Where(s => s.Projects.Any(p => p.Id == projectId))
            .OrderBy(s => s.Category)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }
}
