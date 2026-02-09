# BananaCakePop GraphQL UI Guide

BananaCakePop is the GraphQL IDE that comes with HotChocolate. It's already enabled in this project!

## How to Access

1. **Start the application:**
   ```bash
   # Using Docker Compose (recommended)
   docker-compose up

   # OR using dotnet run
   dotnet run --project CVBackend/CVBackend.csproj
   ```

2. **Open your browser and navigate to:**
   ```
   http://localhost:5000/graphql
   ```

3. **You should see the BananaCakePop UI automatically!**

---

## Quick Start Examples

### 1. Get All Companies
```graphql
query GetAllCompanies {
  companies {
    id
    name
    position
    startDate
    endDate
    description
  }
}
```

### 2. Get Companies with Pagination
```graphql
query GetCompaniesPaged($first: Int!, $after: String) {
  companiesPaged(first: $first, after: $after) {
    edges {
      cursor
      node {
        id
        name
        position
      }
    }
    pageInfo {
      hasNextPage
      hasPreviousPage
      startCursor
      endCursor
    }
  }
}
```

**Variables:**
```json
{
  "first": 10,
  "after": null
}
```

### 3. Get Companies with Projects
```graphql
query GetCompaniesWithProjects {
  companiesWithProjects {
    id
    name
    position
    projects {
      id
      name
      description
      technologies
      startDate
      endDate
    }
  }
}
```

### 4. Get Skills by Category
```graphql
query GetSkillsByCategory($category: String!) {
  skillsByCategory(category: $category) {
    id
    name
    category
    proficiencyLevel
    yearsExperience
  }
}
```

**Variables:**
```json
{
  "category": "Backend"
}
```

### 5. Get Projects with All Relations
```graphql
query GetProjectsWithRelations {
  projectsWithRelations {
    id
    name
    description
    technologies
    company {
      id
      name
      position
    }
    skills {
      id
      name
      category
      proficiencyLevel
    }
  }
}
```

---

## BananaCakePop Features

### 1. **Schema Explorer**
- Click the "Schema" tab to browse all available queries
- See field descriptions and types
- Explore the GraphQL schema visually

### 2. **Query History**
- All your queries are saved automatically
- Access previous queries from the history panel

### 3. **Auto-completion**
- Start typing and get intelligent suggestions
- Press `Ctrl+Space` to trigger autocomplete

### 4. **Variables Panel**
- Define query variables in JSON format
- Variables are strongly typed

### 5. **Documentation**
- Hover over fields to see descriptions
- Click on types to see full documentation

---

## Testing Cache Behavior

Run the same query twice to test caching:

```graphql
query TestCache {
  companies {
    id
    name
  }
}
```

**First run:** Cache miss - fetches from database
**Second run:** Cache hit - returns from cache (within 10 minutes)

Check the logs to see cache hit/miss messages!

---

## GraphQL Introspection

Explore the entire schema:

```graphql
query IntrospectSchema {
  __schema {
    queryType {
      name
      fields {
        name
        description
        type {
          name
          kind
        }
      }
    }
  }
}
```

---

## Tips

1. **Use fragments** to avoid repeating field selections:
   ```graphql
   fragment CompanyFields on Company {
     id
     name
     position
     startDate
     endDate
   }

   query {
     companies {
       ...CompanyFields
     }
   }
   ```

2. **Alias queries** to run multiple queries at once:
   ```graphql
   query MultipleQueries {
     backend: skillsByCategory(category: "Backend") {
       name
     }
     frontend: skillsByCategory(category: "Frontend") {
       name
     }
   }
   ```

3. **Check the Network tab** in browser DevTools to see the actual HTTP requests

---

## Alternative: HTTP Files

If you prefer VSCode/Rider, use the `CVBackend.http` file with the REST Client extension!
