using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CVBackend.Core.Database.Configurations;

/// <summary>
/// Entity Framework Core configuration for the Skill entity.
/// Follows Single Responsibility Principle (SRP).
/// </summary>
public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    /// <summary>
    /// Configures the Skill entity mapping.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("skills");

        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id).HasColumnName("id");

        builder.Property(entity => entity.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(entity => entity.Category)
            .HasColumnName("category")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(entity => entity.ProficiencyLevel)
            .HasColumnName("proficiency_level")
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(entity => entity.YearsExperience)
            .HasColumnName("years_experience")
            .IsRequired();

        builder.HasIndex(entity => entity.Category)
            .HasDatabaseName("idx_skills_category");

        builder.HasIndex(entity => entity.ProficiencyLevel)
            .HasDatabaseName("idx_skills_proficiency_level");

        builder.HasIndex(entity => entity.Name)
            .HasDatabaseName("idx_skills_name");
    }
}
