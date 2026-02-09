using CVBackend.Core.Queries.Implementations;
using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;
using Microsoft.Extensions.Logging.Abstractions;

namespace CVBackend.Tests.Queries;

/// <summary>
/// Unit tests for EducationQuery implementation.
/// </summary>
public class EducationQueryTests : QueryTestBase
{
    private readonly EducationQuery _educationQuery;

    public EducationQueryTests()
    {
        _educationQuery = new EducationQuery(Context, NullLogger<EducationQuery>.Instance);
    }

    [Fact]
    public async Task GetAllAsync_WhenEducationExists_ReturnsAllEducationOrderedByInstitution()
    {
        SeedEducation();

        List<Education> result = await _educationQuery.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("MIT", result[0].Institution);
        Assert.Equal("Stanford University", result[1].Institution);
        Assert.Equal("Tech Academy", result[2].Institution);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoEducation_ReturnsEmptyList()
    {

        List<Education> result = await _educationQuery.GetAllAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEducationExists_ReturnsEducation()
    {
        SeedEducation();
        Guid targetId = Guid.Parse("77777777-7777-7777-7777-777777777777");

        Education? result = await _educationQuery.GetByIdAsync(targetId);

        Assert.NotNull(result);
        Assert.Equal(targetId, result.Id);
        Assert.Equal("MIT", result.Institution);
        Assert.Equal(Enum_DegreeType.Bachelor, result.Degree);
        Assert.Equal("Computer Science", result.Field);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEducationDoesNotExist_ReturnsNull()
    {
        SeedEducation();
        Guid nonExistentId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        Education? result = await _educationQuery.GetByIdAsync(nonExistentId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByDegreeTypeAsync_WhenEducationWithDegreeExists_ReturnsMatchingEducation()
    {
        SeedEducation();

        List<Education> result = await _educationQuery.GetByDegreeTypeAsync(Enum_DegreeType.Bachelor);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("MIT", result[0].Institution);
        Assert.Equal(Enum_DegreeType.Bachelor, result[0].Degree);
    }

    [Fact]
    public async Task GetByDegreeTypeAsync_WhenNoDegreeMatches_ReturnsEmptyList()
    {
        SeedEducation();

        List<Education> result = await _educationQuery.GetByDegreeTypeAsync(Enum_DegreeType.Doctorate);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByDegreeTypeAsync_WhenMultipleMatchingDegrees_ReturnsAllOrderedByInstitution()
    {
        SeedEducation();

        Education additionalBachelor = new Education
        {
            Id = Guid.NewGuid(),
            Institution = "Harvard University",
            Degree = Enum_DegreeType.Bachelor,
            Field = "Mathematics",
            StartDate = new DateTime(2015, 9, 1),
            EndDate = new DateTime(2019, 5, 31)
        };
        Context.Education.Add(additionalBachelor);
        Context.SaveChanges();

        List<Education> result = await _educationQuery.GetByDegreeTypeAsync(Enum_DegreeType.Bachelor);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Harvard University", result[0].Institution); // H comes before M
        Assert.Equal("MIT", result[1].Institution);
    }

    [Fact]
    public async Task GetByInstitutionAsync_WhenInstitutionMatches_ReturnsMatchingEducation()
    {
        SeedEducation();

        List<Education> result = await _educationQuery.GetByInstitutionAsync("MIT");

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("MIT", result[0].Institution);
    }

    [Fact]
    public async Task GetByInstitutionAsync_WhenPartialMatch_ReturnsMatchingEducation()
    {
        SeedEducation();

        List<Education> result = await _educationQuery.GetByInstitutionAsync("University");

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Stanford University", result[0].Institution);
    }

    [Fact]
    public async Task GetByInstitutionAsync_WhenNoMatch_ReturnsEmptyList()
    {
        SeedEducation();

        List<Education> result = await _educationQuery.GetByInstitutionAsync("NonExistent");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByInstitutionAsync_WhenMultipleMatches_ReturnsAllOrderedByInstitution()
    {
        SeedEducation();

        Education additionalEducation = new Education
        {
            Id = Guid.NewGuid(),
            Institution = "Advanced Tech Institute",
            Degree = Enum_DegreeType.Certificate,
            Field = "Data Science",
            StartDate = new DateTime(2021, 1, 1),
            EndDate = new DateTime(2021, 6, 1)
        };
        Context.Education.Add(additionalEducation);
        Context.SaveChanges();

        List<Education> result = await _educationQuery.GetByInstitutionAsync("Tech");

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Advanced Tech Institute", result[0].Institution); // A comes before T
        Assert.Equal("Tech Academy", result[1].Institution);
    }

}
