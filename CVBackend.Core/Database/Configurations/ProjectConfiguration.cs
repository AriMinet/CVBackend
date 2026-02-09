using CVBackend.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CVBackend.Core.Database.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Project entity.
/// Follows Single Responsibility Principle (SRP).
/// </summary>
public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    /// <summary>
    /// Configures the Project entity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).HasColumnName("id");

        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(entity => entity.CompanyId)
            .HasColumnName("company_id");

        builder.Property(entity => entity.Description)
            .HasColumnName("description");

        builder.Property(entity => entity.Technologies)
            .HasColumnName("technologies");

        builder.Property(entity => entity.StartDate)
            .HasColumnName("start_date")
            .IsRequired();

        builder.Property(entity => entity.EndDate)
            .HasColumnName("end_date");

        builder.HasMany(project => project.Skills)
            .WithMany(skill => skill.Projects)
            .UsingEntity(joinEntity =>
            {
                joinEntity.ToTable("project_skills");
            });

        builder.HasIndex(entity => entity.CompanyId)
            .HasDatabaseName("idx_projects_company_id");

        builder.HasIndex(entity => entity.Name)
            .HasDatabaseName("idx_projects_name");

        builder.HasIndex(entity => entity.StartDate)
            .HasDatabaseName("idx_projects_start_date");
    }
}
