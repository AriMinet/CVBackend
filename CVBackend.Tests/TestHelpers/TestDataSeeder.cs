using CVBackend.Shared.Models;
using CVBackend.Shared.Models.Enums;

namespace CVBackend.Tests.TestHelpers;

/// <summary>
/// Centralized test data generator for all test scenarios.
/// Provides consistent test data across unit and integration tests.
/// </summary>
public static class TestDataSeeder
{
    private static class TestGuids
    {
        public static readonly Guid CompanyAlphaCorp = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid CompanyBetaInc = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public static readonly Guid CompanyGammaLlc = Guid.Parse("33333333-3333-3333-3333-333333333333");

        public static readonly Guid ProjectAlpha = Guid.Parse("44444444-4444-4444-4444-444444444444");
        public static readonly Guid ProjectBeta = Guid.Parse("55555555-5555-5555-5555-555555555555");
        public static readonly Guid ProjectGamma = Guid.Parse("66666666-6666-6666-6666-666666666666");

        public static readonly Guid EducationMit = Guid.Parse("77777777-7777-7777-7777-777777777777");
        public static readonly Guid EducationStanford = Guid.Parse("88888888-8888-8888-8888-888888888888");
        public static readonly Guid EducationTechAcademy = Guid.Parse("99999999-9999-9999-9999-999999999999");

        public static readonly Guid SkillCSharp = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        public static readonly Guid SkillReact = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        public static readonly Guid SkillPostgreSql = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        public static readonly Guid SkillDocker = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
    }

    /// <summary>
    /// Creates test companies.
    /// </summary>
    public static List<Company> CreateCompanies()
    {
        return new List<Company>
        {
            new Company
            {
                Id = TestGuids.CompanyAlphaCorp,
                Name = "Alpha Corp",
                Position = "Senior Developer",
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2022, 12, 31),
                Description = "First company"
            },
            new Company
            {
                Id = TestGuids.CompanyBetaInc,
                Name = "Beta Inc",
                Position = "Lead Engineer",
                StartDate = new DateTime(2023, 1, 1),
                EndDate = null,
                Description = "Second company"
            },
            new Company
            {
                Id = TestGuids.CompanyGammaLlc,
                Name = "Gamma LLC",
                Position = "Tech Lead",
                StartDate = new DateTime(2019, 6, 1),
                EndDate = new DateTime(2020, 12, 31),
                Description = "Third company"
            }
        };
    }

    /// <summary>
    /// Creates test skills.
    /// </summary>
    public static List<Skill> CreateSkills()
    {
        return new List<Skill>
        {
            new Skill
            {
                Id = TestGuids.SkillCSharp,
                Name = "C#",
                Category = "Backend",
                ProficiencyLevel = Enum_ProficiencyLevel.Expert,
                YearsExperience = 8
            },
            new Skill
            {
                Id = TestGuids.SkillReact,
                Name = "React",
                Category = "Frontend",
                ProficiencyLevel = Enum_ProficiencyLevel.Advanced,
                YearsExperience = 5
            },
            new Skill
            {
                Id = TestGuids.SkillPostgreSql,
                Name = "PostgreSQL",
                Category = "Database",
                ProficiencyLevel = Enum_ProficiencyLevel.Advanced,
                YearsExperience = 6
            },
            new Skill
            {
                Id = TestGuids.SkillDocker,
                Name = "Docker",
                Category = "Backend",
                ProficiencyLevel = Enum_ProficiencyLevel.Intermediate,
                YearsExperience = 3
            }
        };
    }

    /// <summary>
    /// Creates test projects with linked skills.
    /// NOTE: Skills must be added to context before calling this method.
    /// </summary>
    /// <param name="skills">Skills from database to link to projects.</param>
    public static List<Project> CreateProjects(List<Skill> skills)
    {
        Skill csharpSkill = skills.First(s => s.Name == "C#");
        Skill reactSkill = skills.First(s => s.Name == "React");
        Skill postgresSkill = skills.First(s => s.Name == "PostgreSQL");
        Skill dockerSkill = skills.First(s => s.Name == "Docker");

        return new List<Project>
        {
            new Project
            {
                Id = TestGuids.ProjectAlpha,
                Name = "Project Alpha",
                CompanyId = TestGuids.CompanyAlphaCorp,
                Description = "First project",
                Technologies = "C#, .NET",
                StartDate = new DateTime(2020, 3, 1),
                EndDate = new DateTime(2020, 9, 1),
                Skills = new List<Skill> { csharpSkill, postgresSkill }
            },
            new Project
            {
                Id = TestGuids.ProjectBeta,
                Name = "Project Beta",
                CompanyId = TestGuids.CompanyBetaInc,
                Description = "Second project",
                Technologies = "React, TypeScript",
                StartDate = new DateTime(2023, 2, 1),
                EndDate = null,
                Skills = new List<Skill> { reactSkill }
            },
            new Project
            {
                Id = TestGuids.ProjectGamma,
                Name = "Project Gamma",
                CompanyId = TestGuids.CompanyAlphaCorp,
                Description = "Third project",
                Technologies = "PostgreSQL, Docker",
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2021, 6, 1),
                Skills = new List<Skill> { postgresSkill, dockerSkill, csharpSkill }
            }
        };
    }

    /// <summary>
    /// Creates test education entries.
    /// </summary>
    public static List<Education> CreateEducation()
    {
        return new List<Education>
        {
            new Education
            {
                Id = TestGuids.EducationMit,
                Institution = "MIT",
                Degree = Enum_DegreeType.Bachelor,
                Field = "Computer Science",
                StartDate = new DateTime(2010, 9, 1),
                EndDate = new DateTime(2014, 5, 31),
                Description = "Bachelor degree"
            },
            new Education
            {
                Id = TestGuids.EducationStanford,
                Institution = "Stanford University",
                Degree = Enum_DegreeType.Master,
                Field = "Software Engineering",
                StartDate = new DateTime(2014, 9, 1),
                EndDate = new DateTime(2016, 5, 31),
                Description = "Master degree"
            },
            new Education
            {
                Id = TestGuids.EducationTechAcademy,
                Institution = "Tech Academy",
                Degree = Enum_DegreeType.Certificate,
                Field = "Cloud Computing",
                StartDate = new DateTime(2020, 1, 1),
                EndDate = new DateTime(2020, 6, 1),
                Description = "Certificate program"
            }
        };
    }
}
