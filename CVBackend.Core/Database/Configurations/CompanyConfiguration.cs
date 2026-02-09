using CVBackend.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CVBackend.Core.Database.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Company entity.
/// Follows Single Responsibility Principle (SRP) - one class per entity configuration.
/// </summary>
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    /// <summary>
    /// Configures the Company entity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        // Table mapping
        builder.ToTable("companies");

        // Primary key
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).HasColumnName("id");

        // Properties
        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(entity => entity.Position)
            .HasColumnName("position")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(entity => entity.StartDate)
            .HasColumnName("start_date")
            .IsRequired();

        builder.Property(entity => entity.EndDate)
            .HasColumnName("end_date");

        builder.Property(entity => entity.Description)
            .HasColumnName("description");

        // Relationships
        // Configure one-to-many relationship between Company and Projects
        builder.HasMany(company => company.Projects)
            .WithOne(project => project.Company)
            .HasForeignKey(project => project.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(entity => entity.StartDate)
            .HasDatabaseName("idx_companies_start_date");

        builder.HasIndex(entity => entity.Name)
            .HasDatabaseName("idx_companies_name");
    }
}
