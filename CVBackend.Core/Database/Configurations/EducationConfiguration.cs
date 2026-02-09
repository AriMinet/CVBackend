using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CVBackend.Core.Database.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Education entity.
/// Follows Single Responsibility Principle (SRP).
/// </summary>
public class EducationConfiguration : IEntityTypeConfiguration<Education>
{
    /// <summary>
    /// Configures the Education entity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Education> builder)
    {
        // Table mapping
        builder.ToTable("education");

        // Primary key
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).HasColumnName("id");

        // Properties
        builder.Property(entity => entity.Institution)
            .HasColumnName("institution")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(entity => entity.Degree)
            .HasColumnName("degree")
            .IsRequired()
            .HasConversion<string>()  // Store enum as string in database
            .HasMaxLength(200);

        builder.Property(entity => entity.Field)
            .HasColumnName("field")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(entity => entity.StartDate)
            .HasColumnName("start_date")
            .IsRequired();

        builder.Property(entity => entity.EndDate)
            .HasColumnName("end_date");

        builder.Property(entity => entity.Description)
            .HasColumnName("description");

        // Indexes
        builder.HasIndex(entity => entity.StartDate)
            .HasDatabaseName("idx_education_start_date");

        builder.HasIndex(entity => entity.Institution)
            .HasDatabaseName("idx_education_institution");

        builder.HasIndex(entity => entity.Degree)
            .HasDatabaseName("idx_education_degree");
    }
}
