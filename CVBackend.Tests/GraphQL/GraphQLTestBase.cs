using System.Text;
using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;
using CVBackend.Tests.TestHelpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CVBackend.Tests.GraphQL;

/// <summary>
/// Base class for GraphQL integration tests providing test server setup.
/// </summary>
public abstract class GraphQLTestBase : IDisposable
{
    protected HttpClient Client { get; }
    protected WebApplicationFactory<Program> Factory { get; }

    protected GraphQLTestBase()
    {
        Factory = new CustomWebApplicationFactory();

        Client = Factory.CreateClient();

        using IServiceScope scope = Factory.Services.CreateScope();
        CvDbContext context = scope.ServiceProvider.GetRequiredService<CvDbContext>();
        SeedTestData(context);
    }

    /// <summary>
    /// Seeds test data into the database.
    /// </summary>
    protected virtual void SeedTestData(CvDbContext context)
    {
        SeedCompanies(context);
        SeedSkills(context);
        SeedProjects(context);
        SeedEducation(context);
    }

    /// <summary>
    /// Executes a GraphQL query and returns the response.
    /// </summary>
    protected async Task<GraphQLResponse> ExecuteGraphQLAsync(string query, object? variables = null)
    {
        GraphQLRequest request = new GraphQLRequest
        {
            Query = query,
            Variables = variables
        };

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
        };

        string json = JsonConvert.SerializeObject(request, settings);

        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await Client.PostAsync("/graphql", content);

        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"GraphQL request failed with status {response.StatusCode}. Response: {responseBody}");
        }

        GraphQLResponse? graphQLResponse = JsonConvert.DeserializeObject<GraphQLResponse>(responseBody, settings);

        return graphQLResponse ?? throw new InvalidOperationException("Failed to deserialize GraphQL response");
    }

    /// <summary>
    /// Seeds test companies into the database.
    /// </summary>
    protected void SeedCompanies(CvDbContext context)
    {
        List<Company> companies = TestDataSeeder.CreateCompanies();
        context.Companies.AddRange(companies);
        context.SaveChanges();
    }

    /// <summary>
    /// Seeds test skills into the database.
    /// </summary>
    protected void SeedSkills(CvDbContext context)
    {
        List<Skill> skills = TestDataSeeder.CreateSkills();
        context.Skills.AddRange(skills);
        context.SaveChanges();
    }

    /// <summary>
    /// Seeds test projects into the database with linked skills.
    /// </summary>
    protected void SeedProjects(CvDbContext context)
    {
        List<Skill> allSkills = context.Skills.ToList();

        List<Project> projects = TestDataSeeder.CreateProjects(allSkills);
        context.Projects.AddRange(projects);
        context.SaveChanges();
    }

    /// <summary>
    /// Seeds test education entries into the database.
    /// </summary>
    protected void SeedEducation(CvDbContext context)
    {
        List<Education> education = TestDataSeeder.CreateEducation();
        context.Education.AddRange(education);
        context.SaveChanges();
    }

    public void Dispose()
    {
        Client.Dispose();
        Factory.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// GraphQL request model.
/// </summary>
public class GraphQLRequest
{
    public string Query { get; set; } = string.Empty;
    public object? Variables { get; set; }
}

/// <summary>
/// GraphQL response model.
/// </summary>
public class GraphQLResponse
{
    public JObject? Data { get; set; }
    public List<GraphQLError>? Errors { get; set; }
}

/// <summary>
/// GraphQL error model.
/// </summary>
public class GraphQLError
{
    public string Message { get; set; } = string.Empty;
    public List<JObject>? Locations { get; set; }
    public List<object>? Path { get; set; }
}

/// <summary>
/// Custom WebApplicationFactory for integration tests.
/// Sets Test environment and provides a shared InMemory database name.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public string DatabaseName { get; } = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureServices(services =>
        {
            ServiceDescriptor? descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<CvDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<CvDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName: DatabaseName);
            });
        });
    }
}
