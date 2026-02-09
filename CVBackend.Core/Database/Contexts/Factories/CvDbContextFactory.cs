using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CVBackend.Core.Database.Contexts.Factories;

/// <summary>
/// Factory for creating CvDbContext instances at design time for EF Core migrations.
/// Required for dotnet ef commands to work properly.
/// </summary>
public class CvDbContextFactory : IDesignTimeDbContextFactory<CvDbContext>
{
    /// <summary>
    /// Creates a new instance of CvDbContext for design-time operations.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>A configured CvDbContext instance.</returns>
    public CvDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<CvDbContext> optionsBuilder = new DbContextOptionsBuilder<CvDbContext>();

        string connectionString = "Host=localhost;Port=5432;Database=cvdatabase;Username=cvuser;Password=cvpassword";

        optionsBuilder.UseNpgsql(connectionString);

        return new CvDbContext(optionsBuilder.Options);
    }
}
