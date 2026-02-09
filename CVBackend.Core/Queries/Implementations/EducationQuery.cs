using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;
using CVBackend.Shared.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CVBackend.Core.Queries.Implementations;

/// <summary>
/// Implementation of education-related queries.
/// </summary>
public class EducationQuery : IEducationQuery
{
    private readonly CvDbContext _context;

    /// <summary>
    /// Initializes a new instance of the EducationQuery class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public EducationQuery(CvDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<List<Education>> GetAllAsync()
    {
        return await _context.Education
            .OrderBy(e => e.Institution)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Education?> GetByIdAsync(Guid id)
    {
        return await _context.Education
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    /// <inheritdoc />
    public async Task<List<Education>> GetByDegreeTypeAsync(Enum_DegreeType degreeType)
    {
        return await _context.Education
            .Where(e => e.Degree == degreeType)
            .OrderBy(e => e.Institution)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<List<Education>> GetByInstitutionAsync(string institution)
    {
        return await _context.Education
            .Where(e => e.Institution.Contains(institution))
            .OrderBy(e => e.Institution)
            .ToListAsync();
    }
}
