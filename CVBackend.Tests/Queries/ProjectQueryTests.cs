using CVBackend.Core.Queries.Implementations;
using CVBackend.Shared.Models;

namespace CVBackend.Tests.Queries;

/// <summary>
/// Unit tests for ProjectQuery implementation.
/// </summary>
public class ProjectQueryTests : QueryTestBase
{
    private readonly ProjectQuery _projectQuery;

    public ProjectQueryTests()
    {
        _projectQuery = new ProjectQuery(Context);
    }

    [Fact]
    public async Task GetAllAsync_WhenProjectsExist_ReturnsAllProjectsOrderedByName()
    {
        // Arrange
        SeedProjects();

        // Act
        List<Project> result = await _projectQuery.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("Project Alpha", result[0].Name);
        Assert.Equal("Project Beta", result[1].Name);
        Assert.Equal("Project Gamma", result[2].Name);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoProjects_ReturnsEmptyList()
    {
        // Arrange
        // No data seeded

        // Act
        List<Project> result = await _projectQuery.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectExists_ReturnsProject()
    {
        // Arrange
        SeedProjects();
        Guid targetId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        // Act
        Project? result = await _projectQuery.GetByIdAsync(targetId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(targetId, result.Id);
        Assert.Equal("Project Alpha", result.Name);
        Assert.Equal("First project", result.Description);
        Assert.Equal("C#, .NET", result.Technologies);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectDoesNotExist_ReturnsNull()
    {
        // Arrange
        SeedProjects();
        Guid nonExistentId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        // Act
        Project? result = await _projectQuery.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllWithRelationsAsync_WhenProjectsExist_ReturnsProjectsWithCompanyAndSkillsIncluded()
    {
        // Arrange
        SeedProjects(); // Also seeds companies

        // Act
        List<Project> result = await _projectQuery.GetAllWithRelationsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        // Verify ordering by name
        Assert.Equal("Project Alpha", result[0].Name);
        Assert.Equal("Project Beta", result[1].Name);
        Assert.Equal("Project Gamma", result[2].Name);

        // Verify company is loaded
        Project projectAlpha = result[0];
        Assert.NotNull(projectAlpha.Company);
        Assert.Equal("Alpha Corp", projectAlpha.Company.Name);

        // Verify skills collection exists (may be empty if not seeded)
        Assert.NotNull(projectAlpha.Skills);
    }

    [Fact]
    public async Task GetAllWithRelationsAsync_WhenNoProjects_ReturnsEmptyList()
    {
        // Arrange
        // No data seeded

        // Act
        List<Project> result = await _projectQuery.GetAllWithRelationsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByCompanyIdAsync_WhenCompanyHasProjects_ReturnsProjectsForThatCompany()
    {
        // Arrange
        SeedProjects();
        Guid alphaCorpId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Act
        List<Project> result = await _projectQuery.GetByCompanyIdAsync(alphaCorpId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        // Verify all projects belong to Alpha Corp
        Assert.All(result, p => Assert.Equal(alphaCorpId, p.CompanyId));

        // Verify ordering by name
        Assert.Equal("Project Alpha", result[0].Name);
        Assert.Equal("Project Gamma", result[1].Name);
    }

    [Fact]
    public async Task GetByCompanyIdAsync_WhenCompanyHasNoProjects_ReturnsEmptyList()
    {
        // Arrange
        SeedProjects();
        Guid gammaLlcId = Guid.Parse("33333333-3333-3333-3333-333333333333"); // Has no projects

        // Act
        List<Project> result = await _projectQuery.GetByCompanyIdAsync(gammaLlcId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByCompanyIdAsync_WhenCompanyDoesNotExist_ReturnsEmptyList()
    {
        // Arrange
        SeedProjects();
        Guid nonExistentCompanyId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        // Act
        List<Project> result = await _projectQuery.GetByCompanyIdAsync(nonExistentCompanyId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByCompanyIdAsync_WhenMultipleProjectsForCompany_ReturnsAllOrderedByName()
    {
        // Arrange
        SeedProjects();
        Guid alphaCorpId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Act
        List<Project> result = await _projectQuery.GetByCompanyIdAsync(alphaCorpId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        // Verify correct ordering
        for (int i = 0; i < result.Count - 1; i++)
        {
            Assert.True(string.Compare(result[i].Name, result[i + 1].Name, StringComparison.Ordinal) <= 0,
                $"Projects should be ordered by name: {result[i].Name} should come before {result[i + 1].Name}");
        }
    }

    [Fact]
    public async Task GetBySkillIdAsync_WhenSkillUsedInProjects_ReturnsMatchingProjectsOrderedByName()
    {
        // Arrange
        SeedProjects(); // Also seeds companies and skills
        Guid csharpSkillId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        // Act
        List<Project> result = await _projectQuery.GetBySkillIdAsync(csharpSkillId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        // Verify correct projects are returned (C# is used in Project Alpha and Project Gamma)
        Assert.Contains(result, p => p.Name == "Project Alpha");
        Assert.Contains(result, p => p.Name == "Project Gamma");

        // Verify ordering by name
        Assert.Equal("Project Alpha", result[0].Name);
        Assert.Equal("Project Gamma", result[1].Name);
    }

    [Fact]
    public async Task GetBySkillIdAsync_WhenSkillUsedInSingleProject_ReturnsSingleProject()
    {
        // Arrange
        SeedProjects();
        Guid reactSkillId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        // Act
        List<Project> result = await _projectQuery.GetBySkillIdAsync(reactSkillId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Project Beta", result[0].Name);
    }

    [Fact]
    public async Task GetBySkillIdAsync_WhenSkillUsedInMultipleProjects_ReturnsAllProjectsOrdered()
    {
        // Arrange
        SeedProjects();
        Guid postgresSkillId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        // Act
        List<Project> result = await _projectQuery.GetBySkillIdAsync(postgresSkillId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        // Verify all projects using PostgreSQL are returned
        Assert.Contains(result, p => p.Name == "Project Alpha");
        Assert.Contains(result, p => p.Name == "Project Gamma");

        // Verify ordering by name
        Assert.Equal("Project Alpha", result[0].Name);
        Assert.Equal("Project Gamma", result[1].Name);
    }

    [Fact]
    public async Task GetBySkillIdAsync_WhenSkillNotUsed_ReturnsEmptyList()
    {
        // Arrange
        SeedProjects();

        // Add a skill that's not used in any project
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

        // Act
        List<Project> result = await _projectQuery.GetBySkillIdAsync(unusedSkill.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBySkillIdAsync_WhenSkillDoesNotExist_ReturnsEmptyList()
    {
        // Arrange
        SeedProjects();
        Guid nonExistentSkillId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        // Act
        List<Project> result = await _projectQuery.GetBySkillIdAsync(nonExistentSkillId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBySkillIdAsync_WhenNoData_ReturnsEmptyList()
    {
        // Arrange
        Guid someSkillId = Guid.NewGuid();

        // Act
        List<Project> result = await _projectQuery.GetBySkillIdAsync(someSkillId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
