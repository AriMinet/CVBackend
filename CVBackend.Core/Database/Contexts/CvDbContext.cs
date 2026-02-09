using CVBackend.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace CVBackend.Core.Database.Contexts;

/// <summary>
/// Database context for the CV Backend application.
/// Follows Open/Closed Principle (OCP) - open for extension via configurations,
/// closed for modification.
/// </summary>
public class CvDbContext(DbContextOptions<CvDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the Companies table.
    /// </summary>
    public DbSet<Company> Companies { get; set; }

    /// <summary>
    /// Gets or sets the Projects table.
    /// </summary>
    public DbSet<Project> Projects { get; set; }

    /// <summary>
    /// Gets or sets the Education table.
    /// </summary>
    public DbSet<Education> Education { get; set; }

    /// <summary>
    /// Gets or sets the Skills table.
    /// </summary>
    public DbSet<Skill> Skills { get; set; }

    /// <summary>
    /// Configures the database schema using the model builder.
    /// Uses IEntityTypeConfiguration for better organization (SRP).
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CvDbContext).Assembly);
    }
}
