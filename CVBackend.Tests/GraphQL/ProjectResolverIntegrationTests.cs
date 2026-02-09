using Newtonsoft.Json.Linq;

namespace CVBackend.Tests.GraphQL;

/// <summary>
/// Integration tests for ProjectResolver GraphQL queries.
/// </summary>
public class ProjectResolverIntegrationTests : GraphQLTestBase
{
    [Fact]
    public async Task Projects_ReturnsAllProjects()
    {
        // Arrange
        string query = @"
            query {
                projects {
                    id
                    name
                    description
                    technologies
                    companyId
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projects"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Equal(3, projectsArray.Count);

        // Verify ordering by name
        Assert.Equal("Project Alpha", projectsArray[0]["name"]?.ToString());
        Assert.Equal("Project Beta", projectsArray[1]["name"]?.ToString());
        Assert.Equal("Project Gamma", projectsArray[2]["name"]?.ToString());
    }

    [Fact]
    public async Task Project_WhenProjectExists_ReturnsProjectById()
    {
        // Arrange
        string query = @"
            query {
                project(id: ""44444444-4444-4444-4444-444444444444"") {
                    id
                    name
                    description
                    technologies
                    companyId
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JToken? projectToken = response.Data?["project"];
        Assert.NotNull(projectToken);
        Assert.Equal("44444444-4444-4444-4444-444444444444", projectToken["id"]?.ToString());
        Assert.Equal("Project Alpha", projectToken["name"]?.ToString());
        Assert.Equal("First project", projectToken["description"]?.ToString());
        Assert.Equal("C#, .NET", projectToken["technologies"]?.ToString());
        Assert.Equal("11111111-1111-1111-1111-111111111111", projectToken["companyId"]?.ToString());
    }

    [Fact]
    public async Task Project_WhenProjectDoesNotExist_ReturnsNull()
    {
        // Arrange
        string query = @"
            query {
                project(id: ""ffffffff-ffff-ffff-ffff-ffffffffffff"") {
                    id
                    name
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JToken? projectToken = response.Data?["project"];
        Assert.True(projectToken == null || projectToken.Type == JTokenType.Null);
    }

    [Fact]
    public async Task ProjectsWithRelations_ReturnsProjectsWithCompanyAndSkills()
    {
        // Arrange
        string query = @"
            query {
                projectsWithRelations {
                    id
                    name
                    company {
                        id
                        name
                        position
                    }
                    skills {
                        id
                        name
                        category
                    }
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projectsWithRelations"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Equal(3, projectsArray.Count);

        // Verify first project has company loaded
        JToken firstProject = projectsArray[0];
        Assert.Equal("Project Alpha", firstProject["name"]?.ToString());

        JToken? company = firstProject["company"];
        Assert.NotNull(company);
        Assert.Equal("Alpha Corp", company["name"]?.ToString());
        Assert.Equal("Senior Developer", company["position"]?.ToString());

        // Verify skills are loaded
        JArray? skills = firstProject["skills"] as JArray;
        Assert.NotNull(skills);
        Assert.Equal(2, skills.Count);
    }

    [Fact]
    public async Task ProjectsByCompany_WhenCompanyHasProjects_ReturnsMatchingProjects()
    {
        // Arrange
        string query = @"
            query {
                projectsByCompany(companyId: ""11111111-1111-1111-1111-111111111111"") {
                    id
                    name
                    companyId
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projectsByCompany"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Equal(2, projectsArray.Count);

        // Verify all projects belong to Alpha Corp
        foreach (JToken project in projectsArray)
        {
            Assert.Equal("11111111-1111-1111-1111-111111111111", project["companyId"]?.ToString());
        }

        // Verify ordering by name
        Assert.Equal("Project Alpha", projectsArray[0]["name"]?.ToString());
        Assert.Equal("Project Gamma", projectsArray[1]["name"]?.ToString());
    }

    [Fact]
    public async Task ProjectsByCompany_WhenCompanyHasNoProjects_ReturnsEmptyArray()
    {
        // Arrange
        string query = @"
            query {
                projectsByCompany(companyId: ""33333333-3333-3333-3333-333333333333"") {
                    id
                    name
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projectsByCompany"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Empty(projectsArray);
    }

    [Fact]
    public async Task ProjectsByCompany_WhenCompanyDoesNotExist_ReturnsEmptyArray()
    {
        // Arrange
        string query = @"
            query {
                projectsByCompany(companyId: ""ffffffff-ffff-ffff-ffff-ffffffffffff"") {
                    id
                    name
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projectsByCompany"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Empty(projectsArray);
    }

    [Fact]
    public async Task ProjectsBySkill_WhenSkillUsedInProjects_ReturnsMatchingProjects()
    {
        // Arrange
        string query = @"
            query {
                projectsBySkill(skillId: ""aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"") {
                    id
                    name
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projectsBySkill"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Equal(2, projectsArray.Count);

        // Verify correct projects (C# is used in Project Alpha and Project Gamma)
        Assert.Equal("Project Alpha", projectsArray[0]["name"]?.ToString());
        Assert.Equal("Project Gamma", projectsArray[1]["name"]?.ToString());
    }

    [Fact]
    public async Task ProjectsBySkill_WhenSkillUsedInSingleProject_ReturnsSingleProject()
    {
        // Arrange
        string query = @"
            query {
                projectsBySkill(skillId: ""bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"") {
                    id
                    name
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projectsBySkill"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Single(projectsArray);

        // Verify React is only used in Project Beta
        Assert.Equal("Project Beta", projectsArray[0]["name"]?.ToString());
    }

    [Fact]
    public async Task ProjectsBySkill_WhenSkillNotUsed_ReturnsEmptyArray()
    {
        // Arrange
        string query = @"
            query {
                projectsBySkill(skillId: ""ffffffff-ffff-ffff-ffff-ffffffffffff"") {
                    id
                    name
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projectsBySkill"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Empty(projectsArray);
    }

    [Fact]
    public async Task Projects_WithNestedSkillsQuery_ReturnsProjectsWithSkills()
    {
        // Arrange
        string query = @"
            query {
                projects {
                    id
                    name
                    skills {
                        id
                        name
                        category
                    }
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projects"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Equal(3, projectsArray.Count);

        // Verify Project Alpha has 2 skills
        JToken projectAlpha = projectsArray[0];
        Assert.Equal("Project Alpha", projectAlpha["name"]?.ToString());

        JArray? skills = projectAlpha["skills"] as JArray;
        Assert.NotNull(skills);
        Assert.Equal(2, skills.Count);
    }

    [Fact]
    public async Task ProjectsPaged_WithPagination_ReturnsConnectionWithEdgesAndNodes()
    {
        // Arrange
        string query = @"
            query {
                projectsPaged(first: 2) {
                    edges {
                        node {
                            id
                            name
                            description
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

        JToken? projectsPaged = response.Data?["projectsPaged"];
        Assert.NotNull(projectsPaged);

        // Verify edges
        JArray? edges = projectsPaged["edges"] as JArray;
        Assert.NotNull(edges);
        Assert.Equal(2, edges.Count);

        JToken firstEdge = edges[0];
        Assert.NotNull(firstEdge["node"]);
        Assert.NotNull(firstEdge["cursor"]);
        Assert.Equal("Project Alpha", firstEdge["node"]?["name"]?.ToString());

        // Verify nodes
        JArray? nodes = projectsPaged["nodes"] as JArray;
        Assert.NotNull(nodes);
        Assert.Equal(2, nodes.Count);

        // Verify pageInfo
        JToken? pageInfo = projectsPaged["pageInfo"];
        Assert.NotNull(pageInfo);
        Assert.True(pageInfo["hasNextPage"]?.Value<bool>());
        Assert.False(pageInfo["hasPreviousPage"]?.Value<bool>());
        Assert.NotNull(pageInfo["startCursor"]);
        Assert.NotNull(pageInfo["endCursor"]);
    }
}
