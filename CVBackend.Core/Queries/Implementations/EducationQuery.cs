using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;
using CVBackend.Shared.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CVBackend.Core.Queries.Implementations;

/// <summary>
/// Implementation of education-related queries.
/// </summary>
public class EducationQuery : BaseQuery, IEducationQuery
{
    /// <summary>
    /// Initializes a new instance of the EducationQuery class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="configuration">The configuration instance.</param>
    public EducationQuery(CvDbContext context, ILogger<EducationQuery> logger, IMemoryCache cache, IConfiguration configuration)
        : base(context, logger, cache, configuration)
    {
    }

    /// <inheritdoc />
    public async Task<List<Education>> GetAllAsync()
    {
        string cacheKey = "education_all";

        if (_cachingEnabled && _cache.TryGetValue(cacheKey, out List<Education>? cachedEducation))
        {
            _logger.LogInformation("Cache hit - returning {Count} cached education entries", cachedEducation!.Count);
            return cachedEducation;
        }

        _logger.LogInformation("Cache miss - fetching all education entries from database");
        List<Education> education = await _context.Education
            .OrderBy(e => e.Institution)
            .ToListAsync();

        if (_cachingEnabled)
        {
            MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheExpirationMinutes)
            };
            _cache.Set(cacheKey, education, cacheOptions);
            _logger.LogInformation("Cached {Count} education entries for {Minutes} minutes", education.Count, _cacheExpirationMinutes);
            return education;
        }

        _logger.LogInformation("Retrieved {Count} education entries (caching disabled)", education.Count);

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
