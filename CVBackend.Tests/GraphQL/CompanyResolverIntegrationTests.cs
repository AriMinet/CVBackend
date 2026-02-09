using Newtonsoft.Json.Linq;

namespace CVBackend.Tests.GraphQL;

/// <summary>
/// Integration tests for CompanyResolver GraphQL queries.
/// </summary>
public class CompanyResolverIntegrationTests : GraphQLTestBase
{
    [Fact]
    public async Task Companies_ReturnsAllCompanies()
    {
        // Arrange
        string query = @"
            query {
                companies {
                    id
                    name
                    position
                    description
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? companiesArray = response.Data?["companies"] as JArray;
        Assert.NotNull(companiesArray);
        Assert.Equal(3, companiesArray.Count);

        // Verify first company (ordered by name: Alpha Corp)
        JToken firstCompany = companiesArray[0];
        Assert.Equal("Alpha Corp", firstCompany["name"]?.ToString());
        Assert.Equal("Senior Developer", firstCompany["position"]?.ToString());
        Assert.Equal("First company", firstCompany["description"]?.ToString());
    }

    [Fact]
    public async Task Company_WhenCompanyExists_ReturnsCompanyById()
    {
        // Arrange
        string query = @"
            query {
                company(id: ""11111111-1111-1111-1111-111111111111"") {
                    id
                    name
                    position
                    description
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JToken? companyToken = response.Data?["company"];
        Assert.NotNull(companyToken);
        Assert.Equal("11111111-1111-1111-1111-111111111111", companyToken["id"]?.ToString());
        Assert.Equal("Alpha Corp", companyToken["name"]?.ToString());
        Assert.Equal("Senior Developer", companyToken["position"]?.ToString());
        Assert.Equal("First company", companyToken["description"]?.ToString());
    }

    [Fact]
    public async Task Company_WhenCompanyDoesNotExist_ReturnsNull()
    {
        // Arrange
        string query = @"
            query {
                company(id: ""ffffffff-ffff-ffff-ffff-ffffffffffff"") {
                    id
                    name
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JToken? companyToken = response.Data?["company"];
        Assert.True(companyToken == null || companyToken.Type == JTokenType.Null);
    }

    [Fact]
    public async Task CompaniesWithProjects_ReturnsCompaniesWithProjectsIncluded()
    {
        // Arrange
        string query = @"
            query {
                companiesWithProjects {
                    id
                    name
                    position
                    projects {
                        id
                        name
                        description
                    }
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? companiesArray = response.Data?["companiesWithProjects"] as JArray;
        Assert.NotNull(companiesArray);
        Assert.Equal(3, companiesArray.Count);

        // Verify first company has projects loaded
        JToken firstCompany = companiesArray[0];
        Assert.Equal("Alpha Corp", firstCompany["name"]?.ToString());

        JArray? projects = firstCompany["projects"] as JArray;
        Assert.NotNull(projects);
        Assert.Equal(2, projects.Count);

        // Verify project names
        Assert.Equal("Project Alpha", projects[0]["name"]?.ToString());
        Assert.Equal("Project Gamma", projects[1]["name"]?.ToString());
    }

    [Fact]
    public async Task CompaniesWithProjects_VerifiesCompanyWithNoProjects()
    {
        // Arrange
        string query = @"
            query {
                companiesWithProjects {
                    id
                    name
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

        JArray? companiesArray = response.Data?["companiesWithProjects"] as JArray;
        Assert.NotNull(companiesArray);

        // Find Gamma LLC which has no projects
        JToken? gammaCompany = null;
        foreach (JToken company in companiesArray)
        {
            if (company["name"]?.ToString() == "Gamma LLC")
            {
                gammaCompany = company;
                break;
            }
        }

        Assert.NotNull(gammaCompany);
        JArray? projects = gammaCompany["projects"] as JArray;
        Assert.NotNull(projects);
        Assert.Empty(projects);
    }

    [Fact]
    public async Task Companies_WithDateFields_ReturnsCorrectDates()
    {
        // Arrange
        string query = @"
            query {
                companies {
                    id
                    name
                    startDate
                    endDate
                }
            }";

        // Act
        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? companiesArray = response.Data?["companies"] as JArray;
        Assert.NotNull(companiesArray);

        // Verify Alpha Corp dates
        JToken alphaCompany = companiesArray[0];
        Assert.Equal("Alpha Corp", alphaCompany["name"]?.ToString());
        Assert.Equal("2020-01-01", DateTime.Parse(alphaCompany["startDate"]?.ToString() ?? string.Empty).ToString("yyyy-MM-dd"));
        Assert.Equal("2022-12-31", DateTime.Parse(alphaCompany["endDate"]?.ToString() ?? string.Empty).ToString("yyyy-MM-dd"));

        // Verify Beta Inc has null endDate (still working there)
        JToken betaCompany = companiesArray[1];
        Assert.Equal("Beta Inc", betaCompany["name"]?.ToString());

        JToken? endDate = betaCompany["endDate"];
        Assert.True(endDate == null || endDate.Type == JTokenType.Null);
    }

    [Fact]
    public async Task Companies_NestedProjectsQuery_ReturnsProjectsForEachCompany()
    {
        // Arrange
        string query = @"
            query {
                companies {
                    id
                    name
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

        JArray? companiesArray = response.Data?["companies"] as JArray;
        Assert.NotNull(companiesArray);
        Assert.Equal(3, companiesArray.Count);

        // Verify Alpha Corp has 2 projects
        JToken alphaCompany = companiesArray[0];
        JArray? alphaProjects = alphaCompany["projects"] as JArray;
        Assert.NotNull(alphaProjects);
        Assert.Equal(2, alphaProjects.Count);

        // Verify Beta Inc has 1 project
        JToken betaCompany = companiesArray[1];
        JArray? betaProjects = betaCompany["projects"] as JArray;
        Assert.NotNull(betaProjects);
        Assert.Single(betaProjects);

        // Verify Gamma LLC has 0 projects
        JToken gammaCompany = companiesArray[2];
        JArray? gammaProjects = gammaCompany["projects"] as JArray;
        Assert.NotNull(gammaProjects);
        Assert.Empty(gammaProjects);
    }

    [Fact]
    public async Task CompaniesPaged_WithPagination_ReturnsConnectionWithEdgesAndNodes()
    {
        // Arrange
        string query = @"
            query {
                companiesPaged(first: 2) {
                    edges {
                        node {
                            id
                            name
                            position
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

        JToken? companiesPaged = response.Data?["companiesPaged"];
        Assert.NotNull(companiesPaged);

        // Verify edges
        JArray? edges = companiesPaged["edges"] as JArray;
        Assert.NotNull(edges);
        Assert.Equal(2, edges.Count);

        JToken firstEdge = edges[0];
        Assert.NotNull(firstEdge["node"]);
        Assert.NotNull(firstEdge["cursor"]);
        Assert.Equal("Alpha Corp", firstEdge["node"]?["name"]?.ToString());

        // Verify nodes
        JArray? nodes = companiesPaged["nodes"] as JArray;
        Assert.NotNull(nodes);
        Assert.Equal(2, nodes.Count);

        // Verify pageInfo
        JToken? pageInfo = companiesPaged["pageInfo"];
        Assert.NotNull(pageInfo);
        Assert.True(pageInfo["hasNextPage"]?.Value<bool>());
        Assert.False(pageInfo["hasPreviousPage"]?.Value<bool>());
        Assert.NotNull(pageInfo["startCursor"]);
        Assert.NotNull(pageInfo["endCursor"]);
    }

    [Fact]
    public async Task CompaniesPaged_WithAfterCursor_ReturnsNextPage()
    {
        // Arrange - First query to get cursor
        string firstQuery = @"
            query {
                companiesPaged(first: 1) {
                    edges {
                        cursor
                    }
                    pageInfo {
                        endCursor
                    }
                }
            }";

        GraphQLResponse firstResponse = await ExecuteGraphQLAsync(firstQuery);
        string? endCursor = firstResponse.Data?["companiesPaged"]?["pageInfo"]?["endCursor"]?.ToString();
        Assert.NotNull(endCursor);

        // Act - Second query with after cursor
        string secondQuery = $@"
            query {{
                companiesPaged(first: 1, after: ""{endCursor}"") {{
                    edges {{
                        node {{
                            name
                        }}
                    }}
                    pageInfo {{
                        hasNextPage
                        hasPreviousPage
                    }}
                }}
            }}";

        GraphQLResponse response = await ExecuteGraphQLAsync(secondQuery);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? edges = response.Data?["companiesPaged"]?["edges"] as JArray;
        Assert.NotNull(edges);
        Assert.Single(edges);
        Assert.Equal("Beta Inc", edges[0]["node"]?["name"]?.ToString());

        JToken? pageInfo = response.Data?["companiesPaged"]?["pageInfo"];
        Assert.NotNull(pageInfo);
        Assert.True(pageInfo["hasNextPage"]?.Value<bool>());
        Assert.True(pageInfo["hasPreviousPage"]?.Value<bool>());
    }
}
