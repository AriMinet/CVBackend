using CVBackend.Core.Queries.Implementations;
using CVBackend.Shared.Models;

namespace CVBackend.Tests.Queries;

/// <summary>
/// Unit tests for CompanyQuery implementation.
/// </summary>
public class CompanyQueryTests : QueryTestBase
{
    private readonly CompanyQuery _companyQuery;

    public CompanyQueryTests()
    {
        _companyQuery = new CompanyQuery(Context);
    }

    [Fact]
    public async Task GetAllAsync_WhenCompaniesExist_ReturnsAllCompaniesOrderedByName()
    {
        // Arrange
        SeedCompanies();

        // Act
        List<Company> result = await _companyQuery.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("Alpha Corp", result[0].Name);
        Assert.Equal("Beta Inc", result[1].Name);
        Assert.Equal("Gamma LLC", result[2].Name);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoCompanies_ReturnsEmptyList()
    {
        // Arrange
        // No data seeded

        // Act
        List<Company> result = await _companyQuery.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCompanyExists_ReturnsCompany()
    {
        // Arrange
        SeedCompanies();
        Guid targetId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Act
        Company? result = await _companyQuery.GetByIdAsync(targetId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(targetId, result.Id);
        Assert.Equal("Alpha Corp", result.Name);
        Assert.Equal("Senior Developer", result.Position);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCompanyDoesNotExist_ReturnsNull()
    {
        // Arrange
        SeedCompanies();
        Guid nonExistentId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        // Act
        Company? result = await _companyQuery.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllWithProjectsAsync_WhenCompaniesExist_ReturnsCompaniesWithProjectsIncluded()
    {
        // Arrange
        SeedProjects(); // Also seeds companies

        // Act
        List<Company> result = await _companyQuery.GetAllWithProjectsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        // Verify ordering by name
        Assert.Equal("Alpha Corp", result[0].Name);
        Assert.Equal("Beta Inc", result[1].Name);
        Assert.Equal("Gamma LLC", result[2].Name);

        // Verify projects are loaded (Alpha Corp has 2 projects)
        Company alphaCompany = result[0];
        Assert.NotNull(alphaCompany.Projects);
        Assert.Equal(2, alphaCompany.Projects.Count);
        Assert.Contains(alphaCompany.Projects, p => p.Name == "Project Alpha");
        Assert.Contains(alphaCompany.Projects, p => p.Name == "Project Gamma");
    }

    [Fact]
    public async Task GetAllWithProjectsAsync_WhenNoCompanies_ReturnsEmptyList()
    {
        // Arrange
        // No data seeded

        // Act
        List<Company> result = await _companyQuery.GetAllWithProjectsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdWithProjectsAsync_WhenCompanyExists_ReturnsCompanyWithProjects()
    {
        // Arrange
        SeedProjects(); // Also seeds companies
        Guid targetId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Act
        Company? result = await _companyQuery.GetByIdWithProjectsAsync(targetId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(targetId, result.Id);
        Assert.Equal("Alpha Corp", result.Name);

        // Verify projects are loaded
        Assert.NotNull(result.Projects);
        Assert.Equal(2, result.Projects.Count);
    }

    [Fact]
    public async Task GetByIdWithProjectsAsync_WhenCompanyDoesNotExist_ReturnsNull()
    {
        // Arrange
        SeedProjects();
        Guid nonExistentId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        // Act
        Company? result = await _companyQuery.GetByIdWithProjectsAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdWithProjectsAsync_WhenCompanyHasNoProjects_ReturnsCompanyWithEmptyProjectsList()
    {
        // Arrange
        SeedCompanies();
        Guid targetId = Guid.Parse("33333333-3333-3333-3333-333333333333"); // Gamma LLC has no projects

        // Act
        Company? result = await _companyQuery.GetByIdWithProjectsAsync(targetId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Gamma LLC", result.Name);
        Assert.NotNull(result.Projects);
        Assert.Empty(result.Projects);
    }
}
