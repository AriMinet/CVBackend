using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;
using CVBackend.Shared.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CVBackend.Core.Queries.Implementations;

/// <summary>
/// Implementation of education-related queries.
/// </summary>
public class EducationQuery : IEducationQuery
{
    private readonly CvDbContext _context;
    private readonly ILogger<EducationQuery> _logger;

    /// <summary>
    /// Initializes a new instance of the EducationQuery class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public EducationQuery(CvDbContext context, ILogger<EducationQuery> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<Education>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all education entries");
        List<Education> education = await _context.Education
            .OrderBy(e => e.Institution)
            .ToListAsync();
        _logger.LogInformation("Retrieved {Count} education entries", education.Count);
        return education;
    }

    /// <inheritdoc />
    public async Task<Education?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching education with ID: {EducationId}", id);
        Education? education = await _context.Education
            .FirstOrDefaultAsync(e => e.Id == id);
        if (education == null)
            _logger.LogWarning("Education not found: {EducationId}", id);
        return education;
    }

    /// <inheritdoc />
    public async Task<List<Education>> GetByDegreeTypeAsync(Enum_DegreeType degreeType)
    {
        _logger.LogInformation("Fetching education by degree type: {DegreeType}", degreeType);
        List<Education> education = await _context.Education
            .Where(e => e.Degree == degreeType)
            .OrderBy(e => e.Institution)
            .ToListAsync();
        _logger.LogInformation("Retrieved {Count} education entries for degree: {DegreeType}", education.Count, degreeType);
        return education;
    }

    /// <inheritdoc />
    public async Task<List<Education>> GetByInstitutionAsync(string institution)
    {
        _logger.LogInformation("Fetching education by institution: {Institution}", institution);
        List<Education> education = await _context.Education
            .Where(e => e.Institution.Contains(institution))
            .OrderBy(e => e.Institution)
            .ToListAsync();
        _logger.LogInformation("Retrieved {Count} education entries for institution: {Institution}", education.Count, institution);
        return education;
    }
}
