using CVBackend.Core.Queries.Implementations;
using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;

namespace CVBackend.Tests.Queries;

/// <summary>
/// Unit tests for SkillQuery implementation.
/// </summary>
public class SkillQueryTests : QueryTestBase
{
    private readonly SkillQuery _skillQuery;

    public SkillQueryTests()
    {
        _skillQuery = new SkillQuery(Context);
    }

    [Fact]
    public async Task GetAllAsync_WhenSkillsExist_ReturnsAllSkillsOrderedByCategoryThenName()
    {
        // Arrange
        SeedSkills();

        // Act
        List<Skill> result = await _skillQuery.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count);

        // Verify ordering: Backend category first (C#, Docker), then Database, then Frontend
        Assert.Equal("Backend", result[0].Category);
        Assert.Equal("C#", result[0].Name);
        Assert.Equal("Backend", result[1].Category);
        Assert.Equal("Docker", result[1].Name);
        Assert.Equal("Database", result[2].Category);
        Assert.Equal("PostgreSQL", result[2].Name);
        Assert.Equal("Frontend", result[3].Category);
        Assert.Equal("React", result[3].Name);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoSkills_ReturnsEmptyList()
    {
        // Arrange
        // No data seeded

        // Act
        List<Skill> result = await _skillQuery.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSkillExists_ReturnsSkill()
    {
        // Arrange
        SeedSkills();
        Guid targetId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        // Act
        Skill? result = await _skillQuery.GetByIdAsync(targetId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(targetId, result.Id);
        Assert.Equal("C#", result.Name);
        Assert.Equal("Backend", result.Category);
        Assert.Equal(Enum_ProficiencyLevel.Expert, result.ProficiencyLevel);
        Assert.Equal(8, result.YearsExperience);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSkillDoesNotExist_ReturnsNull()
    {
        // Arrange
        SeedSkills();
        Guid nonExistentId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        // Act
        Skill? result = await _skillQuery.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCategoryAsync_WhenCategoryHasSkills_ReturnsMatchingSkillsOrderedByName()
    {
        // Arrange
        SeedSkills();

        // Act
        List<Skill> result = await _skillQuery.GetByCategoryAsync("Backend");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        // Verify all skills are Backend
        Assert.All(result, s => Assert.Equal("Backend", s.Category));

        // Verify ordering by name
        Assert.Equal("C#", result[0].Name);
        Assert.Equal("Docker", result[1].Name);
    }

    [Fact]
    public async Task GetByCategoryAsync_WhenCategoryHasNoSkills_ReturnsEmptyList()
    {
        // Arrange
        SeedSkills();

        // Act
        List<Skill> result = await _skillQuery.GetByCategoryAsync("NonExistentCategory");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByCategoryAsync_IsCaseSensitive()
    {
        // Arrange
        SeedSkills();

        // Act
        List<Skill> result = await _skillQuery.GetByCategoryAsync("backend"); // lowercase

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result); // Should not match "Backend" (uppercase B)
    }

    [Fact]
    public async Task GetByProficiencyLevelAsync_WhenProficiencyHasSkills_ReturnsMatchingSkillsOrderedByName()
    {
        // Arrange
        SeedSkills();

        // Act
        List<Skill> result = await _skillQuery.GetByProficiencyLevelAsync(Enum_ProficiencyLevel.Advanced);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        // Verify all skills have ADVANCED proficiency
        Assert.All(result, s => Assert.Equal(Enum_ProficiencyLevel.Advanced, s.ProficiencyLevel));

        // Verify ordering by name
        Assert.Equal("PostgreSQL", result[0].Name);
        Assert.Equal("React", result[1].Name);
    }

    [Fact]
    public async Task GetByProficiencyLevelAsync_WhenProficiencyHasNoSkills_ReturnsEmptyList()
    {
        // Arrange
        SeedSkills();

        // Act
        List<Skill> result = await _skillQuery.GetByProficiencyLevelAsync(Enum_ProficiencyLevel.Beginner);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByProficiencyLevelAsync_WhenMultipleProficiencyLevels_ReturnsOnlyMatchingLevel()
    {
        // Arrange
        SeedSkills();

        // Act
        List<Skill> expertResult = await _skillQuery.GetByProficiencyLevelAsync(Enum_ProficiencyLevel.Expert);
        List<Skill> intermediateResult = await _skillQuery.GetByProficiencyLevelAsync(Enum_ProficiencyLevel.Intermediate);

        // Assert
        Assert.Single(expertResult);
        Assert.Equal("C#", expertResult[0].Name);

        Assert.Single(intermediateResult);
        Assert.Equal("Docker", intermediateResult[0].Name);
    }

    [Fact]
    public async Task GetAllWithProjectsAsync_WhenSkillsExist_ReturnsSkillsWithProjectsIncluded()
    {
        // Arrange
        SeedSkills();

        // Act
        List<Skill> result = await _skillQuery.GetAllWithProjectsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count);

        // Verify ordering by category then name
        Assert.Equal("Backend", result[0].Category);
        Assert.Equal("C#", result[0].Name);

        // Verify projects collection exists (may be empty if not seeded)
        Assert.NotNull(result[0].Projects);
    }

    [Fact]
    public async Task GetAllWithProjectsAsync_WhenNoSkills_ReturnsEmptyList()
    {
        // Arrange
        // No data seeded

        // Act
        List<Skill> result = await _skillQuery.GetAllWithProjectsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllWithProjectsAsync_VerifiesOrderingByCategoryThenName()
    {
        // Arrange
        SeedSkills();

        // Add more skills in different categories to test ordering
        List<Skill> additionalSkills = new List<Skill>
        {
            new Skill
            {
                Id = Guid.NewGuid(),
                Name = "Angular",
                Category = "Frontend",
                ProficiencyLevel = Enum_ProficiencyLevel.Intermediate,
                YearsExperience = 2
            },
            new Skill
            {
                Id = Guid.NewGuid(),
                Name = "MongoDB",
                Category = "Database",
                ProficiencyLevel = Enum_ProficiencyLevel.Intermediate,
                YearsExperience = 3
            }
        };
        Context.Skills.AddRange(additionalSkills);
        Context.SaveChanges();

        // Act
        List<Skill> result = await _skillQuery.GetAllWithProjectsAsync();

        // Assert
        Assert.Equal(6, result.Count);

        // Verify primary ordering by category
        Assert.Equal("Backend", result[0].Category);
        Assert.Equal("Backend", result[1].Category);
        Assert.Equal("Database", result[2].Category);
        Assert.Equal("Database", result[3].Category);
        Assert.Equal("Frontend", result[4].Category);
        Assert.Equal("Frontend", result[5].Category);

        // Verify secondary ordering by name within each category
        Assert.Equal("C#", result[0].Name); // Backend: C# before Docker
        Assert.Equal("Docker", result[1].Name);
        Assert.Equal("MongoDB", result[2].Name); // Database: MongoDB before PostgreSQL
        Assert.Equal("PostgreSQL", result[3].Name);
        Assert.Equal("Angular", result[4].Name); // Frontend: Angular before React
        Assert.Equal("React", result[5].Name);
    }

    [Fact]
    public async Task GetByProjectIdAsync_WhenProjectHasSkills_ReturnsMatchingSkillsOrderedByCategoryThenName()
    {
        // Arrange
        SeedProjects(); // Also seeds companies and skills
        Guid projectAlphaId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        // Act
        List<Skill> result = await _skillQuery.GetByProjectIdAsync(projectAlphaId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        // Verify correct skills are returned
        Assert.Contains(result, s => s.Name == "C#");
        Assert.Contains(result, s => s.Name == "PostgreSQL");

        // Verify ordering by category then name
        Assert.Equal("Backend", result[0].Category);
        Assert.Equal("C#", result[0].Name);
        Assert.Equal("Database", result[1].Category);
        Assert.Equal("PostgreSQL", result[1].Name);
    }

    [Fact]
    public async Task GetByProjectIdAsync_WhenProjectHasMultipleSkills_ReturnsAllSkillsOrdered()
    {
        // Arrange
        SeedProjects();
        Guid projectGammaId = Guid.Parse("66666666-6666-6666-6666-666666666666");

        // Act
        List<Skill> result = await _skillQuery.GetByProjectIdAsync(projectGammaId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        // Verify all skills are present
        Assert.Contains(result, s => s.Name == "C#");
        Assert.Contains(result, s => s.Name == "PostgreSQL");
        Assert.Contains(result, s => s.Name == "Docker");

        // Verify ordering by category then name
        Assert.Equal("Backend", result[0].Category); // C# (Backend)
        Assert.Equal("C#", result[0].Name);
        Assert.Equal("Backend", result[1].Category); // Docker (Backend)
        Assert.Equal("Docker", result[1].Name);
        Assert.Equal("Database", result[2].Category); // PostgreSQL (Database)
        Assert.Equal("PostgreSQL", result[2].Name);
    }

    [Fact]
    public async Task GetByProjectIdAsync_WhenProjectHasNoSkills_ReturnsEmptyList()
    {
        // Arrange
        SeedProjects();

        // Add a project with no skills
        Project projectWithoutSkills = new Project
        {
            Id = Guid.NewGuid(),
            Name = "Empty Project",
            CompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Description = "Project with no skills",
            Technologies = "None",
            StartDate = DateTime.Now,
            EndDate = null,
            Skills = new List<Skill>()
        };
        Context.Projects.Add(projectWithoutSkills);
        Context.SaveChanges();

        // Act
        List<Skill> result = await _skillQuery.GetByProjectIdAsync(projectWithoutSkills.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByProjectIdAsync_WhenProjectDoesNotExist_ReturnsEmptyList()
    {
        // Arrange
        SeedProjects();
        Guid nonExistentProjectId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        // Act
        List<Skill> result = await _skillQuery.GetByProjectIdAsync(nonExistentProjectId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByProjectIdAsync_WhenNoData_ReturnsEmptyList()
    {
        // Arrange
        Guid someProjectId = Guid.NewGuid();

        // Act
        List<Skill> result = await _skillQuery.GetByProjectIdAsync(someProjectId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
