using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Models;
using CVBackend.Shared.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CVBackend.Core.Queries.Implementations;

/// <summary>
/// Implementation of company-related queries.
/// </summary>
public class CompanyQuery : ICompanyQuery
{
    private readonly CvDbContext _context;

    /// <summary>
    /// Initializes a new instance of the CompanyQuery class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public CompanyQuery(CvDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<List<Company>> GetAllAsync()
    {
        return await _context.Companies
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Company?> GetByIdAsync(Guid id)
    {
        return await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    /// <inheritdoc />
    public async Task<List<Company>> GetAllWithProjectsAsync()
    {
        List<Company> companies = await _context.Companies
            .Include(c => c.Projects)
            .OrderBy(c => c.Name)
            .ToListAsync();

        // Sort projects within each company
        foreach (Company company in companies)
        {
            company.Projects = company.Projects.OrderBy(p => p.Name).ToList();
        }

        return companies;
    }

    /// <inheritdoc />
    public async Task<Company?> GetByIdWithProjectsAsync(Guid id)
    {
        return await _context.Companies
            .Include(c => c.Projects)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}
