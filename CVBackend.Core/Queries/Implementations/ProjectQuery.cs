using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Models;
using CVBackend.Shared.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CVBackend.Core.Queries.Implementations;

/// <summary>
/// Implementation of project-related queries.
/// </summary>
public class ProjectQuery : IProjectQuery
{
    private readonly CvDbContext _context;

    /// <summary>
    /// Initializes a new instance of the ProjectQuery class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ProjectQuery(CvDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<List<Project>> GetAllAsync()
    {
        return await _context.Projects
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <inheritdoc />
    public async Task<List<Project>> GetAllWithRelationsAsync()
    {
        return await _context.Projects
            .Include(p => p.Company)
            .Include(p => p.Skills)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<Project>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _context.Projects
            .Where(p => p.CompanyId == companyId)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<Project>> GetBySkillIdAsync(Guid skillId)
    {
        return await _context.Projects
            .Where(p => p.Skills.Any(s => s.Id == skillId))
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}
