using CVBackend.Core.Queries.Implementations;
using CVBackend.Shared.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace CVBackend.Tests.Queries;

/// <summary>
/// Unit tests for CompanyQuery implementation.
/// </summary>
public class CompanyQueryTests : QueryTestBase
{
    private readonly CompanyQuery _companyQuery;

    public CompanyQueryTests()
    {
        _companyQuery = new CompanyQuery(Context, NullLogger<CompanyQuery>.Instance, Cache, Configuration);
    }

    private CompanyQuery CreateCompanyQueryWithCachingEnabled()
    {
        Dictionary<string, string?> configValues = new Dictionary<string, string?>
        {
            { "Cache:EnableCaching", "true" },
            { "Cache:ExpirationMinutes", "10" }
        };
        IConfiguration cacheEnabledConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        return new CompanyQuery(Context, NullLogger<CompanyQuery>.Instance, Cache, cacheEnabledConfig);
    }

    [Fact]
    public async Task GetAllAsync_WhenCompaniesExist_ReturnsAllCompaniesOrderedByName()
    {
        SeedCompanies();

        List<Company> result = await _companyQuery.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("Alpha Corp", result[0].Name);
        Assert.Equal("Beta Inc", result[1].Name);
        Assert.Equal("Gamma LLC", result[2].Name);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoCompanies_ReturnsEmptyList()
    {

        List<Company> result = await _companyQuery.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCompanyExists_ReturnsCompany()
    {
        SeedCompanies();
        Guid targetId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        Company? result = await _companyQuery.GetByIdAsync(targetId);

        Assert.NotNull(result);
        Assert.Equal(targetId, result.Id);
        Assert.Equal("Alpha Corp", result.Name);
        Assert.Equal("Senior Developer", result.Position);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCompanyDoesNotExist_ReturnsNull()
    {
        SeedCompanies();
        Guid nonExistentId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        Company? result = await _companyQuery.GetByIdAsync(nonExistentId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllWithProjectsAsync_WhenCompaniesExist_ReturnsCompaniesWithProjectsIncluded()
    {
        SeedProjects(); // Also seeds companies

        List<Company> result = await _companyQuery.GetAllWithProjectsAsync();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        Assert.Equal("Alpha Corp", result[0].Name);
        Assert.Equal("Beta Inc", result[1].Name);
        Assert.Equal("Gamma LLC", result[2].Name);

        Company alphaCompany = result[0];
        Assert.NotNull(alphaCompany.Projects);
        Assert.Equal(2, alphaCompany.Projects.Count);
        Assert.Contains(alphaCompany.Projects, p => p.Name == "Project Alpha");
        Assert.Contains(alphaCompany.Projects, p => p.Name == "Project Gamma");
    }

    [Fact]
    public async Task GetAllWithProjectsAsync_WhenNoCompanies_ReturnsEmptyList()
    {

        List<Company> result = await _companyQuery.GetAllWithProjectsAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdWithProjectsAsync_WhenCompanyExists_ReturnsCompanyWithProjects()
    {
        SeedProjects(); // Also seeds companies
        Guid targetId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        Company? result = await _companyQuery.GetByIdWithProjectsAsync(targetId);

        Assert.NotNull(result);
        Assert.Equal(targetId, result.Id);
        Assert.Equal("Alpha Corp", result.Name);

        Assert.NotNull(result.Projects);
        Assert.Equal(2, result.Projects.Count);
    }

    [Fact]
    public async Task GetByIdWithProjectsAsync_WhenCompanyDoesNotExist_ReturnsNull()
    {
        SeedProjects();
        Guid nonExistentId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        Company? result = await _companyQuery.GetByIdWithProjectsAsync(nonExistentId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdWithProjectsAsync_WhenCompanyHasNoProjects_ReturnsCompanyWithEmptyProjectsList()
    {
        SeedCompanies();
        Guid targetId = Guid.Parse("33333333-3333-3333-3333-333333333333"); // Gamma LLC has no projects

        Company? result = await _companyQuery.GetByIdWithProjectsAsync(targetId);

        Assert.NotNull(result);
        Assert.Equal("Gamma LLC", result.Name);
        Assert.NotNull(result.Projects);
        Assert.Empty(result.Projects);
    }

    [Fact]
    public async Task GetAllAsync_WithCachingEnabled_CachesResultsOnFirstCall()
    {
        CompanyQuery cachedQuery = CreateCompanyQueryWithCachingEnabled();
        SeedCompanies();

        List<Company> firstResult = await cachedQuery.GetAllAsync();
        List<Company> secondResult = await cachedQuery.GetAllAsync();

        Assert.NotNull(firstResult);
        Assert.NotNull(secondResult);
        Assert.Equal(3, firstResult.Count);
        Assert.Equal(3, secondResult.Count);
        Assert.Equal(firstResult[0].Name, secondResult[0].Name);
    }

    [Fact]
    public async Task GetAllAsync_WithCachingEnabled_ReturnsCachedDataOnSubsequentCalls()
    {
        CompanyQuery cachedQuery = CreateCompanyQueryWithCachingEnabled();
        SeedCompanies();

        List<Company> firstResult = await cachedQuery.GetAllAsync();

        Context.Companies.Add(new Company
        {
            Id = Guid.NewGuid(),
            Name = "Zeta Corp",
            Position = "Developer",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = null,
            Description = "New company added after cache"
        });
        Context.SaveChanges();

        List<Company> secondResult = await cachedQuery.GetAllAsync();

        Assert.Equal(3, firstResult.Count);
        Assert.Equal(3, secondResult.Count);
    }

    [Fact]
    public async Task GetAllWithProjectsAsync_WithCachingEnabled_CachesResultsOnFirstCall()
    {
        CompanyQuery cachedQuery = CreateCompanyQueryWithCachingEnabled();
        SeedProjects();

        List<Company> firstResult = await cachedQuery.GetAllWithProjectsAsync();
        List<Company> secondResult = await cachedQuery.GetAllWithProjectsAsync();

        Assert.NotNull(firstResult);
        Assert.NotNull(secondResult);
        Assert.Equal(3, firstResult.Count);
        Assert.Equal(3, secondResult.Count);
        Assert.Equal(2, firstResult[0].Projects.Count);
        Assert.Equal(2, secondResult[0].Projects.Count);
    }

    [Fact]
    public async Task GetAllWithProjectsAsync_WithCachingEnabled_ReturnsCachedDataOnSubsequentCalls()
    {
        CompanyQuery cachedQuery = CreateCompanyQueryWithCachingEnabled();
        SeedProjects();

        List<Company> firstResult = await cachedQuery.GetAllWithProjectsAsync();

        Context.Companies.Add(new Company
        {
            Id = Guid.NewGuid(),
            Name = "Zeta Corp",
            Position = "Developer",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = null,
            Description = "New company added after cache"
        });
        Context.SaveChanges();

        List<Company> secondResult = await cachedQuery.GetAllWithProjectsAsync();

        Assert.Equal(3, firstResult.Count);
        Assert.Equal(3, secondResult.Count);
    }
}
