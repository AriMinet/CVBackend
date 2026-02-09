using CVBackend.Core.Queries.Implementations;
using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;

namespace CVBackend.Tests.Queries;

/// <summary>
/// Unit tests for EducationQuery implementation.
/// </summary>
public class EducationQueryTests : QueryTestBase
{
    private readonly EducationQuery _educationQuery;

    public EducationQueryTests()
    {
        _educationQuery = new EducationQuery(Context);
    }

    [Fact]
    public async Task GetAllAsync_WhenEducationExists_ReturnsAllEducationOrderedByInstitution()
    {
        // Arrange
        SeedEducation();

        // Act
        List<Education> result = await _educationQuery.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("MIT", result[0].Institution);
        Assert.Equal("Stanford University", result[1].Institution);
        Assert.Equal("Tech Academy", result[2].Institution);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoEducation_ReturnsEmptyList()
    {
        // Arrange
        // No data seeded

        // Act
        List<Education> result = await _educationQuery.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEducationExists_ReturnsEducation()
    {
        // Arrange
        SeedEducation();
        Guid targetId = Guid.Parse("77777777-7777-7777-7777-777777777777");

        // Act
        Education? result = await _educationQuery.GetByIdAsync(targetId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(targetId, result.Id);
        Assert.Equal("MIT", result.Institution);
        Assert.Equal(Enum_DegreeType.Bachelor, result.Degree);
        Assert.Equal("Computer Science", result.Field);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEducationDoesNotExist_ReturnsNull()
    {
        // Arrange
        SeedEducation();
        Guid nonExistentId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        // Act
        Education? result = await _educationQuery.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByDegreeTypeAsync_WhenEducationWithDegreeExists_ReturnsMatchingEducation()
    {
        // Arrange
        SeedEducation();

        // Act
        List<Education> result = await _educationQuery.GetByDegreeTypeAsync(Enum_DegreeType.Bachelor);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("MIT", result[0].Institution);
        Assert.Equal(Enum_DegreeType.Bachelor, result[0].Degree);
    }

    [Fact]
    public async Task GetByDegreeTypeAsync_WhenNoDegreeMatches_ReturnsEmptyList()
    {
        // Arrange
        SeedEducation();

        // Act
        List<Education> result = await _educationQuery.GetByDegreeTypeAsync(Enum_DegreeType.Doctorate);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByDegreeTypeAsync_WhenMultipleMatchingDegrees_ReturnsAllOrderedByInstitution()
    {
        // Arrange
        SeedEducation();

        // Add another BACHELOR degree
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

        // Act
        List<Education> result = await _educationQuery.GetByDegreeTypeAsync(Enum_DegreeType.Bachelor);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Harvard University", result[0].Institution); // H comes before M
        Assert.Equal("MIT", result[1].Institution);
    }

    [Fact]
    public async Task GetByInstitutionAsync_WhenInstitutionMatches_ReturnsMatchingEducation()
    {
        // Arrange
        SeedEducation();

        // Act
        List<Education> result = await _educationQuery.GetByInstitutionAsync("MIT");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("MIT", result[0].Institution);
    }

    [Fact]
    public async Task GetByInstitutionAsync_WhenPartialMatch_ReturnsMatchingEducation()
    {
        // Arrange
        SeedEducation();

        // Act
        List<Education> result = await _educationQuery.GetByInstitutionAsync("University");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Stanford University", result[0].Institution);
    }

    [Fact]
    public async Task GetByInstitutionAsync_WhenNoMatch_ReturnsEmptyList()
    {
        // Arrange
        SeedEducation();

        // Act
        List<Education> result = await _educationQuery.GetByInstitutionAsync("NonExistent");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByInstitutionAsync_WhenMultipleMatches_ReturnsAllOrderedByInstitution()
    {
        // Arrange
        SeedEducation();

        // Add another education with "Tech" in the name
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

        // Act
        List<Education> result = await _educationQuery.GetByInstitutionAsync("Tech");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Advanced Tech Institute", result[0].Institution); // A comes before T
        Assert.Equal("Tech Academy", result[1].Institution);
    }

}
