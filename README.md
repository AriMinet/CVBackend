# CV Backend API

GraphQL API for managing CV/resume data

---

## Deploy Commands

### Docker
```bash
docker-compose up          # Start
docker-compose down        # Stop
docker-compose up --build  # Rebuild
```

### Build & Run
```bash
dotnet build              # Build solution
dotnet run --project CVBackend/CVBackend.csproj
```

### Testing
```bash
dotnet test              # Run all tests
```

---

## Endpoints

| Service | URL |
|---------|-----|
| **GraphQL Playground** | http://localhost:5000/graphql |
| **Health Check** | http://localhost:5000/health |
| **PostgreSQL** | localhost:5433 |

---

## Tech Stack

- **Framework:** ASP.NET Core 9.0
- **GraphQL:** HotChocolate 15.1.12
- **Database:** PostgreSQL 17
- **ORM:** Entity Framework Core 9.0.2
- **Testing:** xUnit + WebApplicationFactory
- **Containerization:** Docker & Docker Compose

---

## Available Queries
#### Note: All queries are only highlighted here for test information purposes. For actual query execution, please use the GraphQL Playground.
### Companies
| Query | Description |
|-------|-------------|
| `companies` | Get all companies |
| `companiesPaged(first: Int, after: String)` | Get companies with pagination |
| `company(id: ID!)` | Get company by ID |
| `companiesWithProjects` | Get companies with projects |

### Projects
| Query | Description |
|-------|-------------|
| `projects` | Get all projects |
| `projectsPaged(first: Int, after: String)` | Get projects with pagination |
| `project(id: ID!)` | Get project by ID |
| `projectsWithRelations` | Get projects with company and skills |
| `projectsByCompany(companyId: ID!)` | Get projects for a company |
| `projectsBySkill(skillId: ID!)` | Get projects using a skill |

### Skills
| Query | Description |
|-------|-------------|
| `skills` | Get all skills |
| `skillsPaged(first: Int, after: String)` | Get skills with pagination |
| `skill(id: ID!)` | Get skill by ID |
| `skillsByCategory(category: String!)` | Filter skills by category |
| `skillsByProficiency(proficiencyLevel: ProficiencyLevel!)` | Filter by proficiency |
| `skillsWithProjects` | Get skills with their projects |
| `skillsByProject(projectId: ID!)` | Get skills used in a project |

### Education
| Query | Description |
|-------|-------------|
| `allEducation` | Get all education entries |
| `allEducationPaged(first: Int, after: String)` | Get education with pagination |
| `education(id: ID!)` | Get education by ID |
| `educationByDegree(degree: DegreeType!)` | Filter by degree type |
| `educationByInstitution(institution: String!)` | Filter by institution |
