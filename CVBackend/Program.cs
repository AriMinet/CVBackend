using CVBackend.Core.Database.Contexts;
using CVBackend.Core.Extensions;
using CVBackend.Shared.Database.Seeders.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Threading.RateLimiting;

namespace CVBackend;

/// <summary>
/// Main entry point for the CVBackend GraphQL API application.
/// Follows Single Responsibility Principle (SRP) - orchestrates application startup.
/// </summary>
public class Program
{
    /// <summary>
    /// Main entry point method for the application.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    public static async Task Main(string[] args)
    {
        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add Serilog
            builder.Host.UseSerilog((context, services, configuration) => configuration
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/cvbackend-.log", rollingInterval: RollingInterval.Day));

            ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

            WebApplication app = builder.Build();

            // Skip database initialization in Test environment
            if (app.Environment.EnvironmentName != "Test")
            {
                await InitializeDatabaseAsync(app.Services);
            }

            ConfigureMiddleware(app);

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application terminated unexpectedly: {ex}");
            throw;
        }
    }

    /// <summary>
    /// Configures application services using extension methods.
    /// Follows Dependency Inversion Principle - delegates to extension methods.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <param name="environment">The web host environment.</param>
    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // Add CVBackend core services (DbContext, Seeder, GraphQL)
        // Use InMemory database for Test environment
        bool useInMemoryDatabase = environment.EnvironmentName == "Test";
        services.AddCVBackendServices(configuration, useInMemoryDatabase);

        // Add CORS with configuration
        services.AddCVBackendCors(configuration);

        // Add rate limiting
        bool enableRateLimiting = configuration.GetValue<bool>("RateLimit:EnableRateLimiting");
        if (enableRateLimiting)
        {
            int permitLimit = configuration.GetValue<int>("RateLimit:PermitLimit");
            int window = configuration.GetValue<int>("RateLimit:Window");

            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Request.Headers.Host.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = permitLimit,
                            Window = TimeSpan.FromSeconds(window),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }));

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });
        }

        // Add enhanced health checks
        services.AddHealthChecks()
            .AddDbContextCheck<CvDbContext>("database");
    }

    /// <summary>
    /// Initializes the database with migrations and seed data.
    /// Follows Single Responsibility Principle - separate initialization logic.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    private static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using IServiceScope scope = serviceProvider.CreateScope();

        CvDbContext context = scope.ServiceProvider.GetRequiredService<CvDbContext>();

        // Only run migrations if not using InMemory database (for testing)
        bool isInMemory = context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory";
        if (!isInMemory)
        {
            await context.Database.MigrateAsync();
        }

        // Seed data using injected seeder (only if not in test environment)
        IDbSeeder? seeder = scope.ServiceProvider.GetService<IDbSeeder>();
        if (seeder != null)
        {
            await seeder.SeedAsync();
        }
    }

    /// <summary>
    /// Configures the HTTP request pipeline middleware.
    /// Follows Single Responsibility Principle - separate middleware configuration.
    /// </summary>
    /// <param name="app">The web application.</param>
    private static void ConfigureMiddleware(WebApplication app)
    {
        // Add Serilog request logging
        app.UseSerilogRequestLogging();

        // Global exception handler
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                IExceptionHandlerFeature? exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
                Exception? exception = exceptionHandler?.Error;

                if (exception != null)
                {
                    Log.Error(exception, "Unhandled exception occurred");
                }

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    error = "An unexpected error occurred. Please try again later.",
                    statusCode = 500
                });
            });
        });

        // Enable CORS
        app.UseCors();

        // Enable rate limiting (if configured)
        if (app.Configuration.GetValue<bool>("RateLimit:EnableRateLimiting"))
        {
            app.UseRateLimiter();
        }

        // Map GraphQL endpoint with Banana Cake Pop UI (HotChocolate)
        // Uses resolver classes with query interfaces (SOLID architecture)
        app.MapGraphQL("/graphql");

        // Map enhanced health check endpoint
        app.MapHealthChecks("/health");
    }
}
