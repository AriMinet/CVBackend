using Newtonsoft.Json.Linq;

namespace CVBackend.Tests.GraphQL;

/// <summary>
/// Integration tests for EducationResolver GraphQL queries.
/// </summary>
public class EducationResolverIntegrationTests : GraphQLTestBase
{
    [Fact]
    public async Task AllEducation_ReturnsAllEducationEntries()
    {
        string query = @"
            query {
                allEducation {
                    id
                    institution
                    degree
                    field
                    description
                }
            }";

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? educationArray = response.Data?["allEducation"] as JArray;
        Assert.NotNull(educationArray);
        Assert.Equal(3, educationArray.Count);

        JToken firstEntry = educationArray[0];
        Assert.Equal("MIT", firstEntry["institution"]?.ToString());
        Assert.Equal("BACHELOR", firstEntry["degree"]?.ToString());
        Assert.Equal("Computer Science", firstEntry["field"]?.ToString());
        Assert.Equal("Bachelor degree", firstEntry["description"]?.ToString());
    }

    [Fact]
    public async Task Education_WhenEducationExists_ReturnsEducationById()
    {
        string query = @"
            query {
                education(id: ""77777777-7777-7777-7777-777777777777"") {
                    id
                    institution
                    degree
                    field
                    description
                }
            }";

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JToken? educationToken = response.Data?["education"];
        Assert.NotNull(educationToken);
        Assert.Equal("77777777-7777-7777-7777-777777777777", educationToken["id"]?.ToString());
        Assert.Equal("MIT", educationToken["institution"]?.ToString());
        Assert.Equal("BACHELOR", educationToken["degree"]?.ToString());
        Assert.Equal("Computer Science", educationToken["field"]?.ToString());
    }

    [Fact]
    public async Task Education_WhenEducationDoesNotExist_ReturnsNull()
    {
        string query = @"
            query {
                education(id: ""ffffffff-ffff-ffff-ffff-ffffffffffff"") {
                    id
                    institution
                }
            }";

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JToken? educationToken = response.Data?["education"];
        Assert.True(educationToken == null || educationToken.Type == JTokenType.Null);
    }

    [Fact]
    public async Task EducationByDegree_WhenDegreeExists_ReturnsMatchingEducation()
    {
        string query = @"
            query {
                educationByDegree(degree: BACHELOR) {
                    id
                    institution
                    degree
                    field
                }
            }";

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? educationArray = response.Data?["educationByDegree"] as JArray;
        Assert.NotNull(educationArray);
        Assert.Single(educationArray);

        JToken bachelorEntry = educationArray[0];
        Assert.Equal("MIT", bachelorEntry["institution"]?.ToString());
        Assert.Equal("BACHELOR", bachelorEntry["degree"]?.ToString());
        Assert.Equal("Computer Science", bachelorEntry["field"]?.ToString());
    }

    [Fact]
    public async Task EducationByDegree_WhenDegreeHasNoEntries_ReturnsEmptyArray()
    {
        string query = @"
            query {
                educationByDegree(degree: DOCTORATE) {
                    id
                    institution
                }
            }";

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? educationArray = response.Data?["educationByDegree"] as JArray;
        Assert.NotNull(educationArray);
        Assert.Empty(educationArray);
    }

    [Fact]
    public async Task EducationByInstitution_WhenInstitutionExists_ReturnsMatchingEducation()
    {
        string query = @"
            query {
                educationByInstitution(institution: ""MIT"") {
                    id
                    institution
                    degree
                    field
                }
            }";

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? educationArray = response.Data?["educationByInstitution"] as JArray;
        Assert.NotNull(educationArray);
        Assert.Single(educationArray);

        JToken mitEntry = educationArray[0];
        Assert.Equal("MIT", mitEntry["institution"]?.ToString());
        Assert.Equal("BACHELOR", mitEntry["degree"]?.ToString());
    }

    [Fact]
    public async Task EducationByInstitution_WhenInstitutionDoesNotExist_ReturnsEmptyArray()
    {
        string query = @"
            query {
                educationByInstitution(institution: ""Harvard"") {
                    id
                    institution
                }
            }";

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? educationArray = response.Data?["educationByInstitution"] as JArray;
        Assert.NotNull(educationArray);
        Assert.Empty(educationArray);
    }

    [Fact]
    public async Task AllEducation_WithDateFields_ReturnsCorrectDates()
    {
        string query = @"
            query {
                allEducation {
                    id
                    institution
                    startDate
                    endDate
                }
            }";

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? educationArray = response.Data?["allEducation"] as JArray;
        Assert.NotNull(educationArray);

        JToken mitEntry = educationArray[0];
        Assert.Equal("MIT", mitEntry["institution"]?.ToString());
        Assert.Equal("2010-09-01", DateTime.Parse(mitEntry["startDate"]?.ToString() ?? string.Empty).ToString("yyyy-MM-dd"));
        Assert.Equal("2014-05-31", DateTime.Parse(mitEntry["endDate"]?.ToString() ?? string.Empty).ToString("yyyy-MM-dd"));
    }

    [Fact]
    public async Task AllEducation_VerifiesOrderingByInstitution()
    {
        string query = @"
            query {
                allEducation {
                    id
                    institution
                }
            }";

        GraphQLResponse response = await ExecuteGraphQLAsync(query);

        Assert.Null(response.Errors);
        Assert.NotNull(response.Data);

        JArray? educationArray = response.Data?["allEducation"] as JArray;
        Assert.NotNull(educationArray);
        Assert.Equal(3, educationArray.Count);

        Assert.Equal("MIT", educationArray[0]["institution"]?.ToString());
        Assert.Equal("Stanford University", educationArray[1]["institution"]?.ToString());
        Assert.Equal("Tech Academy", educationArray[2]["institution"]?.ToString());
    }

    [Fact]
    public async Task EducationByDegree_VerifiesAllDegreeTypes()
    {
        string masterQuery = @"
            query {
                educationByDegree(degree: MASTER) {
                    id
                    institution
                    degree
                }
            }";

        string certificateQuery = @"
            query {
                educationByDegree(degree: CERTIFICATE) {
                    id
                    institution
                    degree
                }
            }";

        GraphQLResponse masterResponse = await ExecuteGraphQLAsync(masterQuery);
        GraphQLResponse certificateResponse = await ExecuteGraphQLAsync(certificateQuery);

        Assert.Null(masterResponse.Errors);
        JArray? masterArray = masterResponse.Data?["educationByDegree"] as JArray;
        Assert.NotNull(masterArray);
        Assert.Single(masterArray);
        Assert.Equal("Stanford University", masterArray[0]["institution"]?.ToString());
        Assert.Equal("MASTER", masterArray[0]["degree"]?.ToString());

        Assert.Null(certificateResponse.Errors);
        JArray? certificateArray = certificateResponse.Data?["educationByDegree"] as JArray;
        Assert.NotNull(certificateArray);
        Assert.Single(certificateArray);
        Assert.Equal("Tech Academy", certificateArray[0]["institution"]?.ToString());
        Assert.Equal("CERTIFICATE", certificateArray[0]["degree"]?.ToString());
    }

    [Fact]
    public async Task AllEducationPaged_WithPagination_ReturnsConnectionWithEdgesAndNodes()
    {
        string query = @"
            query {
                allEducationPaged(first: 2) {
                    edges {
                        node {
                            id
                            institution
                            degree
                            field
                        }
                        cursor
                    }
                    nodes {
                        id
                        institution
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

        JToken? educationPaged = response.Data?["allEducationPaged"];
        Assert.NotNull(educationPaged);

        JArray? edges = educationPaged["edges"] as JArray;
        Assert.NotNull(edges);
        Assert.Equal(2, edges.Count);

        JToken firstEdge = edges[0];
        Assert.NotNull(firstEdge["node"]);
        Assert.NotNull(firstEdge["cursor"]);
        Assert.Equal("MIT", firstEdge["node"]?["institution"]?.ToString());

        JArray? nodes = educationPaged["nodes"] as JArray;
        Assert.NotNull(nodes);
        Assert.Equal(2, nodes.Count);

        JToken? pageInfo = educationPaged["pageInfo"];
        Assert.NotNull(pageInfo);
        Assert.True(pageInfo["hasNextPage"]?.Value<bool>());
        Assert.False(pageInfo["hasPreviousPage"]?.Value<bool>());
        Assert.NotNull(pageInfo["startCursor"]);
        Assert.NotNull(pageInfo["endCursor"]);
    }
}
