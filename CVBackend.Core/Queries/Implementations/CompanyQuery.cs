using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Models;
using CVBackend.Shared.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CVBackend.Core.Queries.Implementations;

/// <summary>
/// Implementation of company-related queries.
/// </summary>
public class CompanyQuery : ICompanyQuery
{
    private readonly CvDbContext _context;
    private readonly ILogger<CompanyQuery> _logger;

    /// <summary>
    /// Initializes a new instance of the CompanyQuery class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public CompanyQuery(CvDbContext context, ILogger<CompanyQuery> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<List<Company>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all companies");
        List<Company> companies = await _context.Companies
            .OrderBy(c => c.Name)
            .ToListAsync();
        _logger.LogInformation("Retrieved {Count} companies", companies.Count);
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
        _logger.LogInformation("Fetching all companies with projects");
        List<Company> companies = await _context.Companies
            .Include(c => c.Projects)
            .OrderBy(c => c.Name)
            .ToListAsync();

        foreach (Company company in companies)
        {
            company.Projects = company.Projects.OrderBy(p => p.Name).ToList();
        }

        _logger.LogInformation("Retrieved {Count} companies with projects", companies.Count);
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
