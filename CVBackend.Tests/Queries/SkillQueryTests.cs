using CVBackend.Core.Queries.Implementations;
using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;
using Microsoft.Extensions.Logging.Abstractions;

namespace CVBackend.Tests.Queries;

/// <summary>
/// Unit tests for SkillQuery implementation.
/// </summary>
public class SkillQueryTests : QueryTestBase
{
    private readonly SkillQuery _skillQuery;

    public SkillQueryTests()
    {
        _skillQuery = new SkillQuery(Context, NullLogger<SkillQuery>.Instance);
    }

    [Fact]
    public async Task GetAllAsync_WhenSkillsExist_ReturnsAllSkillsOrderedByCategoryThenName()
    {
        SeedSkills();

        List<Skill> result = await _skillQuery.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(4, result.Count);

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

        List<Skill> result = await _skillQuery.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSkillExists_ReturnsSkill()
    {
        SeedSkills();
        Guid targetId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        Skill? result = await _skillQuery.GetByIdAsync(targetId);

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
        SeedSkills();
        Guid nonExistentId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        Skill? result = await _skillQuery.GetByIdAsync(nonExistentId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCategoryAsync_WhenCategoryHasSkills_ReturnsMatchingSkillsOrderedByName()
    {
        SeedSkills();

        List<Skill> result = await _skillQuery.GetByCategoryAsync("Backend");

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.All(result, s => Assert.Equal("Backend", s.Category));

        Assert.Equal("C#", result[0].Name);
        Assert.Equal("Docker", result[1].Name);
    }

    [Fact]
    public async Task GetByCategoryAsync_WhenCategoryHasNoSkills_ReturnsEmptyList()
    {
        SeedSkills();

        List<Skill> result = await _skillQuery.GetByCategoryAsync("NonExistentCategory");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByCategoryAsync_IsCaseSensitive()
    {
        SeedSkills();

        List<Skill> result = await _skillQuery.GetByCategoryAsync("backend"); // lowercase

        Assert.NotNull(result);
        Assert.Empty(result); // Should not match "Backend" (uppercase B)
    }

    [Fact]
    public async Task GetByProficiencyLevelAsync_WhenProficiencyHasSkills_ReturnsMatchingSkillsOrderedByName()
    {
        SeedSkills();

        List<Skill> result = await _skillQuery.GetByProficiencyLevelAsync(Enum_ProficiencyLevel.Advanced);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.All(result, s => Assert.Equal(Enum_ProficiencyLevel.Advanced, s.ProficiencyLevel));

        Assert.Equal("PostgreSQL", result[0].Name);
        Assert.Equal("React", result[1].Name);
    }

    [Fact]
    public async Task GetByProficiencyLevelAsync_WhenProficiencyHasNoSkills_ReturnsEmptyList()
    {
        SeedSkills();

        List<Skill> result = await _skillQuery.GetByProficiencyLevelAsync(Enum_ProficiencyLevel.Beginner);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByProficiencyLevelAsync_WhenMultipleProficiencyLevels_ReturnsOnlyMatchingLevel()
    {
        SeedSkills();

        List<Skill> expertResult = await _skillQuery.GetByProficiencyLevelAsync(Enum_ProficiencyLevel.Expert);
        List<Skill> intermediateResult = await _skillQuery.GetByProficiencyLevelAsync(Enum_ProficiencyLevel.Intermediate);

        Assert.Single(expertResult);
        Assert.Equal("C#", expertResult[0].Name);

        Assert.Single(intermediateResult);
        Assert.Equal("Docker", intermediateResult[0].Name);
    }

    [Fact]
    public async Task GetAllWithProjectsAsync_WhenSkillsExist_ReturnsSkillsWithProjectsIncluded()
    {
        SeedSkills();

        List<Skill> result = await _skillQuery.GetAllWithProjectsAsync();

        Assert.NotNull(result);
        Assert.Equal(4, result.Count);

        Assert.Equal("Backend", result[0].Category);
        Assert.Equal("C#", result[0].Name);

        Assert.NotNull(result[0].Projects);
    }

    [Fact]
    public async Task GetAllWithProjectsAsync_WhenNoSkills_ReturnsEmptyList()
    {

        List<Skill> result = await _skillQuery.GetAllWithProjectsAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllWithProjectsAsync_VerifiesOrderingByCategoryThenName()
    {
        SeedSkills();

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

        List<Skill> result = await _skillQuery.GetAllWithProjectsAsync();

        Assert.Equal(6, result.Count);

        Assert.Equal("Backend", result[0].Category);
        Assert.Equal("Backend", result[1].Category);
        Assert.Equal("Database", result[2].Category);
        Assert.Equal("Database", result[3].Category);
        Assert.Equal("Frontend", result[4].Category);
        Assert.Equal("Frontend", result[5].Category);

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
        SeedProjects(); // Also seeds companies and skills
        Guid projectAlphaId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        List<Skill> result = await _skillQuery.GetByProjectIdAsync(projectAlphaId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);

        Assert.Contains(result, s => s.Name == "C#");
        Assert.Contains(result, s => s.Name == "PostgreSQL");

        Assert.Equal("Backend", result[0].Category);
        Assert.Equal("C#", result[0].Name);
        Assert.Equal("Database", result[1].Category);
        Assert.Equal("PostgreSQL", result[1].Name);
    }

    [Fact]
    public async Task GetByProjectIdAsync_WhenProjectHasMultipleSkills_ReturnsAllSkillsOrdered()
    {
        SeedProjects();
        Guid projectGammaId = Guid.Parse("66666666-6666-6666-6666-666666666666");

        List<Skill> result = await _skillQuery.GetByProjectIdAsync(projectGammaId);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);

        Assert.Contains(result, s => s.Name == "C#");
        Assert.Contains(result, s => s.Name == "PostgreSQL");
        Assert.Contains(result, s => s.Name == "Docker");

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
        SeedProjects();

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

        List<Skill> result = await _skillQuery.GetByProjectIdAsync(projectWithoutSkills.Id);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByProjectIdAsync_WhenProjectDoesNotExist_ReturnsEmptyList()
    {
        SeedProjects();
        Guid nonExistentProjectId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        List<Skill> result = await _skillQuery.GetByProjectIdAsync(nonExistentProjectId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByProjectIdAsync_WhenNoData_ReturnsEmptyList()
    {
        Guid someProjectId = Guid.NewGuid();

        List<Skill> result = await _skillQuery.GetByProjectIdAsync(someProjectId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
