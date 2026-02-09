using CVBackend.Core.Queries.Implementations;
using CVBackend.Shared.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace CVBackend.Tests.Queries;

/// <summary>
/// Unit tests for ProjectQuery implementation.
/// </summary>
public class ProjectQueryTests : QueryTestBase
{
    private readonly ProjectQuery _projectQuery;

    public ProjectQueryTests()
    {
        _projectQuery = new ProjectQuery(Context, NullLogger<ProjectQuery>.Instance, Cache, Configuration);
    }

    private ProjectQuery CreateProjectQueryWithCachingEnabled()
    {
        Dictionary<string, string?> configValues = new Dictionary<string, string?>
        {
            { "Cache:EnableCaching", "true" },
            { "Cache:ExpirationMinutes", "10" }
        };
        IConfiguration cacheEnabledConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        return new ProjectQuery(Context, NullLogger<ProjectQuery>.Instance, Cache, cacheEnabledConfig);
    }

    [Fact]
    public async Task GetAllAsync_WhenProjectsExist_ReturnsAllProjectsOrderedByName()
    {
        SeedProjects();

        List<Project> result = await _projectQuery.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("Project Alpha", result[0].Name);
        Assert.Equal("Project Beta", result[1].Name);
        Assert.Equal("Project Gamma", result[2].Name);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoProjects_ReturnsEmptyList()
    {

        List<Project> result = await _projectQuery.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectExists_ReturnsProject()
    {
        SeedProjects();
        Guid targetId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        Project? result = await _projectQuery.GetByIdAsync(targetId);

        Assert.NotNull(result);
        Assert.Equal(targetId, result.Id);
        Assert.Equal("Project Alpha", result.Name);
        Assert.Equal("First project", result.Description);
        Assert.Equal("C#, .NET", result.Technologies);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectDoesNotExist_ReturnsNull()
    {
        SeedProjects();
        Guid nonExistentId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        Project? result = await _projectQuery.GetByIdAsync(nonExistentId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllWithRelationsAsync_WhenProjectsExist_ReturnsProjectsWithCompanyAndSkillsIncluded()
    {
        SeedProjects(); // Also seeds companies

        List<Project> result = await _projectQuery.GetAllWithRelationsAsync();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        Assert.Equal("Project Alpha", result[0].Name);
        Assert.Equal("Project Beta", result[1].Name);
        Assert.Equal("Project Gamma", result[2].Name);

        Project projectAlpha = result[0];
        Assert.NotNull(projectAlpha.Company);
        Assert.Equal("Alpha Corp", projectAlpha.Company.Name);

        Assert.NotNull(projectAlpha.Skills);
    }

    [Fact]
    public async Task GetAllWithRelationsAsync_WhenNoProjects_ReturnsEmptyList()
    {

        List<Project> result = await _projectQuery.GetAllWithRelationsAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByCompanyIdAsync_WhenCompanyHasProjects_ReturnsProjectsForThatCompany()
    {
        SeedProjects();
        Guid alphaCorpId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        List<Project> result = await _projectQuery.GetByCompanyIdAsync(alphaCorpId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.All(result, p => Assert.Equal(alphaCorpId, p.CompanyId));

        Assert.Equal("Project Alpha", result[0].Name);
        Assert.Equal("Project Gamma", result[1].Name);
    }

    [Fact]
    public async Task GetByCompanyIdAsync_WhenCompanyHasNoProjects_ReturnsEmptyList()
    {
        SeedProjects();
        Guid gammaLlcId = Guid.Parse("33333333-3333-3333-3333-333333333333"); // Has no projects

        List<Project> result = await _projectQuery.GetByCompanyIdAsync(gammaLlcId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByCompanyIdAsync_WhenCompanyDoesNotExist_ReturnsEmptyList()
    {
        SeedProjects();
        Guid nonExistentCompanyId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        List<Project> result = await _projectQuery.GetByCompanyIdAsync(nonExistentCompanyId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByCompanyIdAsync_WhenMultipleProjectsForCompany_ReturnsAllOrderedByName()
    {
        SeedProjects();
        Guid alphaCorpId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        List<Project> result = await _projectQuery.GetByCompanyIdAsync(alphaCorpId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        for (int i = 0; i < result.Count - 1; i++)
        {
            Assert.True(string.Compare(result[i].Name, result[i + 1].Name, StringComparison.Ordinal) <= 0,
                $"Projects should be ordered by name: {result[i].Name} should come before {result[i + 1].Name}");
        }
    }

    [Fact]
    public async Task GetBySkillIdAsync_WhenSkillUsedInProjects_ReturnsMatchingProjectsOrderedByName()
    {
        SeedProjects(); // Also seeds companies and skills
        Guid csharpSkillId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        List<Project> result = await _projectQuery.GetBySkillIdAsync(csharpSkillId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Contains(result, p => p.Name == "Project Alpha");
        Assert.Contains(result, p => p.Name == "Project Gamma");

        Assert.Equal("Project Alpha", result[0].Name);
        Assert.Equal("Project Gamma", result[1].Name);
    }

    [Fact]
    public async Task GetBySkillIdAsync_WhenSkillUsedInSingleProject_ReturnsSingleProject()
    {
        SeedProjects();
        Guid reactSkillId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        List<Project> result = await _projectQuery.GetBySkillIdAsync(reactSkillId);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Project Beta", result[0].Name);
    }

    [Fact]
    public async Task GetBySkillIdAsync_WhenSkillUsedInMultipleProjects_ReturnsAllProjectsOrdered()
    {
        SeedProjects();
        Guid postgresSkillId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        List<Project> result = await _projectQuery.GetBySkillIdAsync(postgresSkillId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Contains(result, p => p.Name == "Project Alpha");
        Assert.Contains(result, p => p.Name == "Project Gamma");

        Assert.Equal("Project Alpha", result[0].Name);
        Assert.Equal("Project Gamma", result[1].Name);
    }

    [Fact]
    public async Task GetBySkillIdAsync_WhenSkillNotUsed_ReturnsEmptyList()
    {
        SeedProjects();

        Skill unusedSkill = new Skill
        {
            Id = Guid.NewGuid(),
            Name = "Rust",
            Category = "Backend",
            ProficiencyLevel = Shared.Models.Enums.Enum_ProficiencyLevel.Beginner,
            YearsExperience = 1
        };
        Context.Skills.Add(unusedSkill);
        Context.SaveChanges();

        List<Project> result = await _projectQuery.GetBySkillIdAsync(unusedSkill.Id);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBySkillIdAsync_WhenSkillDoesNotExist_ReturnsEmptyList()
    {
        SeedProjects();
        Guid nonExistentSkillId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        List<Project> result = await _projectQuery.GetBySkillIdAsync(nonExistentSkillId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBySkillIdAsync_WhenNoData_ReturnsEmptyList()
    {
        Guid someSkillId = Guid.NewGuid();

        List<Project> result = await _projectQuery.GetBySkillIdAsync(someSkillId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WithCachingEnabled_CachesResultsOnFirstCall()
    {
        ProjectQuery cachedQuery = CreateProjectQueryWithCachingEnabled();
        SeedProjects();

        List<Project> firstResult = await cachedQuery.GetAllAsync();
        List<Project> secondResult = await cachedQuery.GetAllAsync();

        Assert.NotNull(firstResult);
        Assert.NotNull(secondResult);
        Assert.Equal(3, firstResult.Count);
        Assert.Equal(3, secondResult.Count);
        Assert.Equal(firstResult[0].Name, secondResult[0].Name);
    }

    [Fact]
    public async Task GetAllAsync_WithCachingEnabled_ReturnsCachedDataOnSubsequentCalls()
    {
        ProjectQuery cachedQuery = CreateProjectQueryWithCachingEnabled();
        SeedProjects();

        List<Project> firstResult = await cachedQuery.GetAllAsync();

        Context.Projects.Add(new Project
        {
            Id = Guid.NewGuid(),
            Name = "Project Zeta",
            CompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Description = "New project added after cache",
            Technologies = "New Tech",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = null
        });
        Context.SaveChanges();

        List<Project> secondResult = await cachedQuery.GetAllAsync();

        Assert.Equal(3, firstResult.Count);
        Assert.Equal(3, secondResult.Count);
    }

    [Fact]
    public async Task GetAllWithRelationsAsync_WithCachingEnabled_CachesResultsOnFirstCall()
    {
        ProjectQuery cachedQuery = CreateProjectQueryWithCachingEnabled();
        SeedProjects();

        List<Project> firstResult = await cachedQuery.GetAllWithRelationsAsync();
        List<Project> secondResult = await cachedQuery.GetAllWithRelationsAsync();

        Assert.NotNull(firstResult);
        Assert.NotNull(secondResult);
        Assert.Equal(3, firstResult.Count);
        Assert.Equal(3, secondResult.Count);
        Assert.NotNull(firstResult[0].Company);
        Assert.NotNull(secondResult[0].Company);
    }

    [Fact]
    public async Task GetAllWithRelationsAsync_WithCachingEnabled_ReturnsCachedDataOnSubsequentCalls()
    {
        ProjectQuery cachedQuery = CreateProjectQueryWithCachingEnabled();
        SeedProjects();

        List<Project> firstResult = await cachedQuery.GetAllWithRelationsAsync();

        Context.Projects.Add(new Project
        {
            Id = Guid.NewGuid(),
            Name = "Project Zeta",
            CompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Description = "New project added after cache",
            Technologies = "New Tech",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = null
        });
        Context.SaveChanges();

        List<Project> secondResult = await cachedQuery.GetAllWithRelationsAsync();

        Assert.Equal(3, firstResult.Count);
        Assert.Equal(3, secondResult.Count);
    }
}
