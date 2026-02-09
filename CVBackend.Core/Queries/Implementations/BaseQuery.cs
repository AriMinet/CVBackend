using CVBackend.Core.Database.Contexts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CVBackend.Core.Queries.Implementations;

/// <summary>
/// Base class for all query implementations providing common dependencies.
/// </summary>
public abstract class BaseQuery
{
    protected readonly CvDbContext _context;
    protected readonly ILogger _logger;
    protected readonly IMemoryCache _cache;
    protected readonly IConfiguration _configuration;
    protected readonly bool _cachingEnabled;
    protected readonly int _cacheExpirationMinutes;

    /// <summary>
    /// Initializes a new instance of the BaseQuery class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="configuration">The configuration instance.</param>
    protected BaseQuery(CvDbContext context, ILogger logger, IMemoryCache cache, IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
        _configuration = configuration;
        _cachingEnabled = configuration.GetValue<bool>("Cache:EnableCaching");
        _cacheExpirationMinutes = configuration.GetValue<int>("Cache:ExpirationMinutes");
    }
}
