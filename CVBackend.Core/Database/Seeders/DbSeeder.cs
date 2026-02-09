using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Database.Seeders.Interfaces;
using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CVBackend.Core.Database.Seeders;

/// <summary>
/// Database seeder implementation.
/// Follows Single Responsibility Principle (SRP) - only responsible for seeding data.
/// Follows Dependency Inversion Principle (DIP) - depends on IDbSeeder abstraction.
/// </summary>
public class DbSeeder : IDbSeeder
{
    private readonly CvDbContext _context;
    private readonly ILogger<DbSeeder> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbSeeder"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">The logger instance.</param>
    public DbSeeder(CvDbContext context, ILogger<DbSeeder> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<bool> IsSeededAsync(CancellationToken cancellationToken = default)
    {
        bool hasCompanies = await _context.Companies.AnyAsync(cancellationToken);
        bool hasEducation = await _context.Education.AnyAsync(cancellationToken);
        bool hasSkills = await _context.Skills.AnyAsync(cancellationToken);

        return hasCompanies || hasEducation || hasSkills;
    }

    /// <inheritdoc />
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (await IsSeededAsync(cancellationToken))
            {
                _logger.LogInformation("Database already contains data. Skipping seeding.");
                return;
            }

            _logger.LogInformation("Seeding database with initial data...");

            await SeedCompaniesAsync(cancellationToken);
            await SeedSkillsAsync(cancellationToken);
            await SeedProjectsAsync(cancellationToken);
            await SeedEducationAsync(cancellationToken);

            _logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    /// <summary>
    /// Seeds companies data.
    /// </summary>
    private async Task SeedCompaniesAsync(CancellationToken cancellationToken)
    {
        List<Company> companies = new List<Company>
        {
            new Company
            {
                Name = "Tech Innovations Inc",
                Position = "Senior Software Engineer",
                StartDate = DateTime.SpecifyKind(new DateTime(2020, 3, 1), DateTimeKind.Utc),
                EndDate = null,
                Description = "Led development of microservices architecture and mentored junior developers. Implemented CI/CD pipelines and improved system performance by 40%."
            },
            new Company
            {
                Name = "Digital Solutions Ltd",
                Position = "Full Stack Developer",
                StartDate = DateTime.SpecifyKind(new DateTime(2018, 6, 15), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2020, 2, 28), DateTimeKind.Utc),
                Description = "Developed web applications using React and .NET Core. Worked on database optimization and API design."
            },
            new Company
            {
                Name = "StartupHub",
                Position = "Junior Developer",
                StartDate = DateTime.SpecifyKind(new DateTime(2016, 9, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2018, 6, 1), DateTimeKind.Utc),
                Description = "First professional role. Contributed to various projects using JavaScript, Python, and SQL."
            }
        };

        await _context.Companies.AddRangeAsync(companies, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded {Count} companies.", companies.Count);
    }

    /// <summary>
    /// Seeds projects data with linked skills.
    /// </summary>
    private async Task SeedProjectsAsync(CancellationToken cancellationToken)
    {
        List<Company> companies = await _context.Companies.ToListAsync(cancellationToken);

        if (companies.Count < 3)
        {
            _logger.LogWarning("Not enough companies to seed projects.");
            return;
        }

        List<Skill> allSkills = await _context.Skills.ToListAsync(cancellationToken);

        List<Skill> FindSkills(params string[] skillNames)
        {
            return allSkills.Where(s => skillNames.Contains(s.Name)).ToList();
        }

        List<Project> projects = new List<Project>
        {
            new Project
            {
                Name = "E-Commerce Platform Redesign",
                CompanyId = companies[0].Id,
                Description = "Complete overhaul of legacy e-commerce system with modern tech stack",
                Technologies = "C#, ASP.NET Core, React, PostgreSQL, Docker, Kubernetes",
                StartDate = DateTime.SpecifyKind(new DateTime(2021, 1, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2021, 12, 31), DateTimeKind.Utc),
                Skills = FindSkills("C#", "ASP.NET Core", "React", "PostgreSQL", "Docker", "Kubernetes")
            },
            new Project
            {
                Name = "Real-time Analytics Dashboard",
                CompanyId = companies[0].Id,
                Description = "Built real-time data visualization platform for business intelligence",
                Technologies = "SignalR, GraphQL, React, Redis, TimescaleDB",
                StartDate = DateTime.SpecifyKind(new DateTime(2022, 3, 1), DateTimeKind.Utc),
                EndDate = null,
                Skills = FindSkills("GraphQL", "React", "PostgreSQL")
            },
            new Project
            {
                Name = "Customer Portal Application",
                CompanyId = companies[1].Id,
                Description = "Self-service portal for customers to manage accounts and support tickets",
                Technologies = "React, Node.js, Express, MongoDB, JWT Authentication",
                StartDate = DateTime.SpecifyKind(new DateTime(2019, 1, 15), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2019, 11, 30), DateTimeKind.Utc),
                Skills = FindSkills("React", "TypeScript")
            },
            new Project
            {
                Name = "Inventory Management System",
                CompanyId = companies[1].Id,
                Description = "Cloud-based inventory tracking with barcode scanning integration",
                Technologies = "Angular, .NET Core, SQL Server, Azure",
                StartDate = DateTime.SpecifyKind(new DateTime(2019, 6, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2020, 1, 15), DateTimeKind.Utc),
                Skills = FindSkills("C#", "ASP.NET Core", "SQL Server", "Azure")
            },
            new Project
            {
                Name = "Mobile App Prototype",
                CompanyId = companies[2].Id,
                Description = "Proof of concept for cross-platform mobile application",
                Technologies = "React Native, Firebase, REST APIs",
                StartDate = DateTime.SpecifyKind(new DateTime(2017, 3, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2017, 8, 31), DateTimeKind.Utc),
                Skills = FindSkills("React", "TypeScript")
            }
        };

        await _context.Projects.AddRangeAsync(projects, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded {Count} projects with skills linked.", projects.Count);
    }

    /// <summary>
    /// Seeds education data.
    /// </summary>
    private async Task SeedEducationAsync(CancellationToken cancellationToken)
    {
        List<Education> education = new List<Education>
        {
            new Education
            {
                Institution = "University of Technology",
                Degree = Enum_DegreeType.Bachelor,
                Field = "Computer Science",
                StartDate = DateTime.SpecifyKind(new DateTime(2012, 9, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2016, 6, 30), DateTimeKind.Utc),
                Description = "Graduated with honors. Focus on software engineering, algorithms, and database systems. Final year project: Distributed file storage system."
            },
            new Education
            {
                Institution = "Tech Academy Online",
                Degree = Enum_DegreeType.Professional,
                Field = "Cloud Architecture",
                StartDate = DateTime.SpecifyKind(new DateTime(2021, 1, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2021, 6, 30), DateTimeKind.Utc),
                Description = "Comprehensive course covering AWS, Azure, Docker, Kubernetes, and DevOps practices."
            },
            new Education
            {
                Institution = "Data Science Institute",
                Degree = Enum_DegreeType.Certificate,
                Field = "Machine Learning",
                StartDate = DateTime.SpecifyKind(new DateTime(2022, 9, 1), DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(new DateTime(2023, 3, 31), DateTimeKind.Utc),
                Description = "Focused on practical ML applications, neural networks, and data analysis with Python."
            }
        };

        await _context.Education.AddRangeAsync(education, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded {Count} education entries.", education.Count);
    }

    /// <summary>
    /// Seeds skills data.
    /// </summary>
    private async Task SeedSkillsAsync(CancellationToken cancellationToken)
    {
        List<Skill> skills = new List<Skill>
        {
            new Skill { Name = "C#", Category = "Backend", ProficiencyLevel = Enum_ProficiencyLevel.Expert, YearsExperience = 7 },
            new Skill { Name = "ASP.NET Core", Category = "Backend", ProficiencyLevel = Enum_ProficiencyLevel.Expert, YearsExperience = 6 },
            new Skill { Name = "Entity Framework Core", Category = "Backend", ProficiencyLevel = Enum_ProficiencyLevel.Advanced, YearsExperience = 6 },
            new Skill { Name = "GraphQL", Category = "Backend", ProficiencyLevel = Enum_ProficiencyLevel.Advanced, YearsExperience = 3 },
            new Skill { Name = "React", Category = "Frontend", ProficiencyLevel = Enum_ProficiencyLevel.Advanced, YearsExperience = 5 },
            new Skill { Name = "TypeScript", Category = "Frontend", ProficiencyLevel = Enum_ProficiencyLevel.Advanced, YearsExperience = 4 },
            new Skill { Name = "PostgreSQL", Category = "Database", ProficiencyLevel = Enum_ProficiencyLevel.Advanced, YearsExperience = 6 },
            new Skill { Name = "SQL Server", Category = "Database", ProficiencyLevel = Enum_ProficiencyLevel.Advanced, YearsExperience = 7 },
            new Skill { Name = "Docker", Category = "DevOps", ProficiencyLevel = Enum_ProficiencyLevel.Advanced, YearsExperience = 4 },
            new Skill { Name = "Kubernetes", Category = "DevOps", ProficiencyLevel = Enum_ProficiencyLevel.Intermediate, YearsExperience = 2 },
            new Skill { Name = "Git", Category = "Tools", ProficiencyLevel = Enum_ProficiencyLevel.Expert, YearsExperience = 8 },
            new Skill { Name = "Azure", Category = "Cloud", ProficiencyLevel = Enum_ProficiencyLevel.Intermediate, YearsExperience = 3 }
        };

        await _context.Skills.AddRangeAsync(skills, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded {Count} skills.", skills.Count);
    }
}
