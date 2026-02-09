using CVBackend.Core.Database.Contexts;
using CVBackend.Core.Database.Seeders;
using CVBackend.Core.GraphQL.Resolvers;
using CVBackend.Core.Queries.Implementations;
using CVBackend.Shared.Database.Seeders.Interfaces;
using CVBackend.Shared.Queries.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CVBackend.Core.Extensions;

/// <summary>
/// Extension methods for configuring CVBackend services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds all CVBackend core services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="useInMemoryDatabase">If true, uses InMemory database instead of PostgreSQL (for testing).</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddCVBackendServices(
        this IServiceCollection services,
        IConfiguration configuration,
        bool useInMemoryDatabase = false)
    {
        if (useInMemoryDatabase)
        {
            services.AddDbContext<CvDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });
        }
        else
        {
            string? environment = configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT");
            bool isDevelopment = environment == "Development" || environment == "Test";

            services.AddDbContext<CvDbContext>(options =>
            {
                string? connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null);

                    npgsqlOptions.CommandTimeout(30);
                });

                if (isDevelopment)
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
            });
        }

        services.AddScoped<IDbSeeder, DbSeeder>();

        services.AddMemoryCache();

        services.AddScoped<ICompanyQuery, CompanyQuery>();
        services.AddScoped<IProjectQuery, ProjectQuery>();
        services.AddScoped<IEducationQuery, EducationQuery>();
        services.AddScoped<ISkillQuery, SkillQuery>();

        services
            .AddGraphQLServer()
            .AddQueryType(d => d.Name("Query"))
            .AddProjections()
            .AddFiltering()
            .AddSorting()
            .AddTypeExtension<CompanyResolver>()
            .AddTypeExtension<ProjectResolver>()
            .AddTypeExtension<EducationResolver>()
            .AddTypeExtension<SkillResolver>();

        return services;
    }

    /// <summary>
    /// Adds CORS configuration for the CVBackend API.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddCVBackendCors(this IServiceCollection services, IConfiguration configuration)
    {
        string[]? allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                if (allowedOrigins != null && allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                }
                else
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                }
            });
        });

        return services;
    }
}
