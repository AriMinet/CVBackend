using Newtonsoft.Json.Linq;

namespace CVBackend.Tests.GraphQL;

/// <summary>
/// Integration tests for SkillResolver GraphQL queries.
/// </summary>
public class SkillResolverIntegrationTests : GraphQLTestBase
{
    [Fact]
    public async Task Skills_ReturnsAllSkills()
    {
        // Arrange
        string query = @"
            query {
                skills {
                    id
                    name
                    category
                    proficiencyLevel
                    yearsExperience
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? skillsArray = response.Data?["skills"] as JArray;
        Assert.NotNull(skillsArray);
        Assert.Equal(4, skillsArray.Count);

        // Verify first skill (ordered by category then name: Backend -> C#)
        JToken firstSkill = skillsArray[0];
        Assert.Equal("C#", firstSkill["name"]?.ToString());
        Assert.Equal("Backend", firstSkill["category"]?.ToString());
        Assert.Equal("EXPERT", firstSkill["proficiencyLevel"]?.ToString());
        Assert.Equal(8, firstSkill["yearsExperience"]?.Value<int>());
    }

    [Fact]
    public async Task Skill_WhenSkillExists_ReturnsSkillById()
    {
        // Arrange
        string query = @"
            query {
                skill(id: ""aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"") {
                    id
                    name
                    category
                    proficiencyLevel
                    yearsExperience
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JToken? skillToken = response.Data?["skill"];
        Assert.NotNull(skillToken);
        Assert.Equal("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", skillToken["id"]?.ToString());
        Assert.Equal("C#", skillToken["name"]?.ToString());
        Assert.Equal("Backend", skillToken["category"]?.ToString());
    }

    [Fact]
    public async Task Skill_WhenSkillDoesNotExist_ReturnsNull()
    {
        // Arrange
        string query = @"
            query {
                skill(id: ""ffffffff-ffff-ffff-ffff-ffffffffffff"") {
                    id
                    name
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JToken? skillToken = response.Data?["skill"];
        Assert.True(skillToken == null || skillToken.Type == JTokenType.Null);
    }

    [Fact]
    public async Task SkillsByCategory_WhenCategoryHasSkills_ReturnsMatchingSkills()
    {
        // Arrange
        string query = @"
            query {
                skillsByCategory(category: ""Backend"") {
                    id
                    name
                    category
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? skillsArray = response.Data?["skillsByCategory"] as JArray;
        Assert.NotNull(skillsArray);
        Assert.Equal(2, skillsArray.Count);

        // Verify all are Backend category
        foreach (JToken skill in skillsArray)
        {
            Assert.Equal("Backend", skill["category"]?.ToString());
        }

        // Verify ordering by name
        Assert.Equal("C#", skillsArray[0]["name"]?.ToString());
        Assert.Equal("Docker", skillsArray[1]["name"]?.ToString());
    }

    [Fact]
    public async Task SkillsByCategory_WhenCategoryHasNoSkills_ReturnsEmptyArray()
    {
        // Arrange
        string query = @"
            query {
                skillsByCategory(category: ""NonExistentCategory"") {
                    id
                    name
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? skillsArray = response.Data?["skillsByCategory"] as JArray;
        Assert.NotNull(skillsArray);
        Assert.Empty(skillsArray);
    }

    [Fact]
    public async Task SkillsByProficiency_WhenProficiencyHasSkills_ReturnsMatchingSkills()
    {
        // Arrange
        string query = @"
            query {
                skillsByProficiency(proficiencyLevel: ADVANCED) {
                    id
                    name
                    proficiencyLevel
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? skillsArray = response.Data?["skillsByProficiency"] as JArray;
        Assert.NotNull(skillsArray);
        Assert.Equal(2, skillsArray.Count);

        // Verify all are ADVANCED proficiency
        foreach (JToken skill in skillsArray)
        {
            Assert.Equal("ADVANCED", skill["proficiencyLevel"]?.ToString());
        }
    }

    [Fact]
    public async Task SkillsWithProjects_ReturnsSkillsWithProjectsIncluded()
    {
        // Arrange
        string query = @"
            query {
                skillsWithProjects {
                    id
                    name
                    category
                    projects {
                        id
                        name
                    }
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? skillsArray = response.Data?["skillsWithProjects"] as JArray;
        Assert.NotNull(skillsArray);
        Assert.Equal(4, skillsArray.Count);

        // Verify C# skill has 2 projects (Project Alpha and Project Gamma)
        JToken csharpSkill = skillsArray[0];
        Assert.Equal("C#", csharpSkill["name"]?.ToString());

        JArray? projects = csharpSkill["projects"] as JArray;
        Assert.NotNull(projects);
        Assert.Equal(2, projects.Count);
    }

    [Fact]
    public async Task SkillsByProject_WhenProjectHasSkills_ReturnsMatchingSkills()
    {
        // Arrange
        string query = @"
            query {
                skillsByProject(projectId: ""44444444-4444-4444-4444-444444444444"") {
                    id
                    name
                    category
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? skillsArray = response.Data?["skillsByProject"] as JArray;
        Assert.NotNull(skillsArray);
        Assert.Equal(2, skillsArray.Count);

        // Verify correct skills (C# and PostgreSQL for Project Alpha)
        Assert.Equal("C#", skillsArray[0]["name"]?.ToString());
        Assert.Equal("Backend", skillsArray[0]["category"]?.ToString());
        Assert.Equal("PostgreSQL", skillsArray[1]["name"]?.ToString());
        Assert.Equal("Database", skillsArray[1]["category"]?.ToString());
    }

    [Fact]
    public async Task SkillsByProject_WhenProjectHasMultipleSkills_ReturnsAllOrdered()
    {
        // Arrange
        string query = @"
            query {
                skillsByProject(projectId: ""66666666-6666-6666-6666-666666666666"") {
                    id
                    name
                    category
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? skillsArray = response.Data?["skillsByProject"] as JArray;
        Assert.NotNull(skillsArray);
        Assert.Equal(3, skillsArray.Count);

        // Verify ordering by category then name
        Assert.Equal("C#", skillsArray[0]["name"]?.ToString());
        Assert.Equal("Backend", skillsArray[0]["category"]?.ToString());
        Assert.Equal("Docker", skillsArray[1]["name"]?.ToString());
        Assert.Equal("Backend", skillsArray[1]["category"]?.ToString());
        Assert.Equal("PostgreSQL", skillsArray[2]["name"]?.ToString());
        Assert.Equal("Database", skillsArray[2]["category"]?.ToString());
    }

    [Fact]
    public async Task SkillsByProject_WhenProjectDoesNotExist_ReturnsEmptyArray()
    {
        // Arrange
        string query = @"
            query {
                skillsByProject(projectId: ""ffffffff-ffff-ffff-ffff-ffffffffffff"") {
                    id
                    name
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? skillsArray = response.Data?["skillsByProject"] as JArray;
        Assert.NotNull(skillsArray);
        Assert.Empty(skillsArray);
    }

    [Fact]
    public async Task SkillsPaged_WithPagination_ReturnsConnectionWithEdgesAndNodes()
    {
        // Arrange
        string query = @"
            query {
                skillsPaged(first: 2) {
                    edges {
                        node {
                            id
                            name
                            category
                            proficiencyLevel
                        }
                        cursor
                    }
                    nodes {
                        id
                        name
                    }
                    pageInfo {
                        hasNextPage
                        hasPreviousPage
                        startCursor
                        endCursor
                    }
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JToken? skillsPaged = response.Data?["skillsPaged"];
        Assert.NotNull(skillsPaged);

        // Verify edges
        JArray? edges = skillsPaged["edges"] as JArray;
        Assert.NotNull(edges);
        Assert.Equal(2, edges.Count);

        JToken firstEdge = edges[0];
        Assert.NotNull(firstEdge["node"]);
        Assert.NotNull(firstEdge["cursor"]);
        Assert.Equal("C#", firstEdge["node"]?["name"]?.ToString());

        // Verify nodes
        JArray? nodes = skillsPaged["nodes"] as JArray;
        Assert.NotNull(nodes);
        Assert.Equal(2, nodes.Count);

        // Verify pageInfo
        JToken? pageInfo = skillsPaged["pageInfo"];
        Assert.NotNull(pageInfo);
        Assert.True(pageInfo["hasNextPage"]?.Value<bool>());
        Assert.False(pageInfo["hasPreviousPage"]?.Value<bool>());
        Assert.NotNull(pageInfo["startCursor"]);
        Assert.NotNull(pageInfo["endCursor"]);
    }
}
