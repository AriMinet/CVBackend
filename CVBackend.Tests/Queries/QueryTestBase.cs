using CVBackend.Core.Database.Contexts;
using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;
using CVBackend.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace CVBackend.Tests.Queries;

/// <summary>
/// Base class for query tests providing in-memory database setup.
/// </summary>
public abstract class QueryTestBase : IDisposable
{
    protected CvDbContext Context { get; }

    protected QueryTestBase()
    {
        DbContextOptions<CvDbContext> options = new DbContextOptionsBuilder<CvDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new CvDbContext(options);
    }

    /// <summary>
    /// Seeds test companies into the database.
    /// </summary>
    protected void SeedCompanies()
    {
        List<Company> companies = TestDataSeeder.CreateCompanies();
        Context.Companies.AddRange(companies);
        Context.SaveChanges();
    }

    /// <summary>
    /// Seeds test projects into the database with linked skills.
    /// </summary>
    protected void SeedProjects()
    {
        SeedCompanies(); // Projects need companies
        SeedSkills(); // Projects need skills to link

        // Load skills from database
        List<Skill> allSkills = Context.Skills.ToList();

        List<Project> projects = TestDataSeeder.CreateProjects(allSkills);
        Context.Projects.AddRange(projects);
        Context.SaveChanges();
    }

    /// <summary>
    /// Seeds test education entries into the database.
    /// </summary>
    protected void SeedEducation()
    {
        List<Education> education = TestDataSeeder.CreateEducation();
        Context.Education.AddRange(education);
        Context.SaveChanges();
    }

    /// <summary>
    /// Seeds test skills into the database.
    /// </summary>
    protected void SeedSkills()
    {
        List<Skill> skills = TestDataSeeder.CreateSkills();
        Context.Skills.AddRange(skills);
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
