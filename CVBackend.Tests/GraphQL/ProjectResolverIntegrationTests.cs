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

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projects"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Equal(3, projectsArray.Count);

        Assert.Equal("Project Alpha", projectsArray[0]["name"]?.ToString());
        Assert.Equal("Project Beta", projectsArray[1]["name"]?.ToString());
        Assert.Equal("Project Gamma", projectsArray[2]["name"]?.ToString());
    }

    [Fact]
    public async Task Project_WhenProjectExists_ReturnsProjectById()
    {
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

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

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
        string query = @"
            query {
                project(id: ""ffffffff-ffff-ffff-ffff-ffffffffffff"") {
                    id
                    name
                }
            }";

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JToken? projectToken = response.Data?["project"];
        Assert.True(projectToken == null || projectToken.Type == JTokenType.Null);
    }

    [Fact]
    public async Task ProjectsWithRelations_ReturnsProjectsWithCompanyAndSkills()
    {
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

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projectsWithRelations"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Equal(3, projectsArray.Count);

        JToken firstProject = projectsArray[0];
        Assert.Equal("Project Alpha", firstProject["name"]?.ToString());

        JToken? company = firstProject["company"];
        Assert.NotNull(company);
        Assert.Equal("Alpha Corp", company["name"]?.ToString());
        Assert.Equal("Senior Developer", company["position"]?.ToString());

        JArray? skills = firstProject["skills"] as JArray;
        Assert.NotNull(skills);
        Assert.Equal(2, skills.Count);
    }

    [Fact]
    public async Task ProjectsByCompany_WhenCompanyHasProjects_ReturnsMatchingProjects()
    {
        string query = @"
            query {
                projectsByCompany(companyId: ""11111111-1111-1111-1111-111111111111"") {
                    id
                    name
                    companyId
                }
            }";

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projectsByCompany"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Equal(2, projectsArray.Count);

        foreach (JToken project in projectsArray)
        {
            Assert.Equal("11111111-1111-1111-1111-111111111111", project["companyId"]?.ToString());
        }

        Assert.Equal("Project Alpha", projectsArray[0]["name"]?.ToString());
        Assert.Equal("Project Gamma", projectsArray[1]["name"]?.ToString());
    }

    [Fact]
    public async Task ProjectsByCompany_WhenCompanyHasNoProjects_ReturnsEmptyArray()
    {
        string query = @"
            query {
                projectsByCompany(companyId: ""33333333-3333-3333-3333-333333333333"") {
                    id
                    name
                }
            }";

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projectsByCompany"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Empty(projectsArray);
    }

    [Fact]
    public async Task ProjectsByCompany_WhenCompanyDoesNotExist_ReturnsEmptyArray()
    {
        string query = @"
            query {
                projectsByCompany(companyId: ""ffffffff-ffff-ffff-ffff-ffffffffffff"") {
                    id
                    name
                }
            }";

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projectsByCompany"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Empty(projectsArray);
    }

    [Fact]
    public async Task ProjectsBySkill_WhenSkillUsedInProjects_ReturnsMatchingProjects()
    {
        string query = @"
            query {
                projectsBySkill(skillId: ""aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"") {
                    id
                    name
                }
            }";

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projectsBySkill"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Equal(2, projectsArray.Count);

        Assert.Equal("Project Alpha", projectsArray[0]["name"]?.ToString());
        Assert.Equal("Project Gamma", projectsArray[1]["name"]?.ToString());
    }

    [Fact]
    public async Task ProjectsBySkill_WhenSkillUsedInSingleProject_ReturnsSingleProject()
    {
        string query = @"
            query {
                projectsBySkill(skillId: ""bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"") {
                    id
                    name
                }
            }";

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projectsBySkill"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Single(projectsArray);

        Assert.Equal("Project Beta", projectsArray[0]["name"]?.ToString());
    }

    [Fact]
    public async Task ProjectsBySkill_WhenSkillNotUsed_ReturnsEmptyArray()
    {
        string query = @"
            query {
                projectsBySkill(skillId: ""ffffffff-ffff-ffff-ffff-ffffffffffff"") {
                    id
                    name
                }
            }";

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projectsBySkill"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Empty(projectsArray);
    }

    [Fact]
    public async Task Projects_WithNestedSkillsQuery_ReturnsProjectsWithSkills()
    {
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

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? projectsArray = response.Data?["projects"] as JArray;
        Assert.NotNull(projectsArray);
        Assert.Equal(3, projectsArray.Count);

        JToken projectAlpha = projectsArray[0];
        Assert.Equal("Project Alpha", projectAlpha["name"]?.ToString());

        JArray? skills = projectAlpha["skills"] as JArray;
        Assert.NotNull(skills);
        Assert.Equal(2, skills.Count);
    }

    [Fact]
    public async Task ProjectsPaged_WithPagination_ReturnsConnectionWithEdgesAndNodes()
    {
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

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JToken? projectsPaged = response.Data?["projectsPaged"];
        Assert.NotNull(projectsPaged);

        JArray? edges = projectsPaged["edges"] as JArray;
        Assert.NotNull(edges);
        Assert.Equal(2, edges.Count);

        JToken firstEdge = edges[0];
        Assert.NotNull(firstEdge["node"]);
        Assert.NotNull(firstEdge["cursor"]);
        Assert.Equal("Project Alpha", firstEdge["node"]?["name"]?.ToString());

        JArray? nodes = projectsPaged["nodes"] as JArray;
        Assert.NotNull(nodes);
        Assert.Equal(2, nodes.Count);

        JToken? pageInfo = projectsPaged["pageInfo"];
        Assert.NotNull(pageInfo);
        Assert.True(pageInfo["hasNextPage"]?.Value<bool>());
        Assert.False(pageInfo["hasPreviousPage"]?.Value<bool>());
        Assert.NotNull(pageInfo["startCursor"]);
        Assert.NotNull(pageInfo["endCursor"]);
    }
}
