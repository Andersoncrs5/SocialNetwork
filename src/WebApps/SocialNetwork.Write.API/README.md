# Social Network Write API

This is the Write side of the CQRS architecture. It is responsible for handling commands, enforcing 
business rules, and maintaining data integrity within the primary distributed database (TiDB).

## Technologies

* **Runtime**: .NET 10
* **Data Access**: Entity Framework Core with Fluent API configuration
* **Design Patterns**:
    * Unit of Work (Centralized repository management)
    * Facade (Simplifying service interactions)
    * Repository Pattern (Data abstraction)
* **Principles**: SOLID, Dry, and Clean Code
* **Storage**: AWS S3 (Media and static assets)

---

## Project Structure

The project is organized by responsibility to ensure high maintainability:
    
    .
    ├── Annotations         # Custom validation attributes (Category, Post, Tag, User)
    ├── Configs             # System configurations
    │   ├── DB              # DbContext and database initialization
    │   ├── Exception       # Global Exception Handling and custom exception classes
    │   └── InfoApp         # Application metadata and environment settings
    ├── Controllers         # API Endpoints (Command handlers)
    ├── dto                 # Data Transfer Objects organized by domain
    ├── Migrations          # Entity Framework database migrations
    ├── Models              # Domain entities
    │   ├── Bases           # Base classes (BaseModel, etc.)
    │   └── Enums           # Domain-specific enumerations (Post status, visibility, etc.)
    ├── Repositories        # Data access layer
    │   ├── Interfaces      # Repository contracts
    │   └── Provider        # Entity Framework implementations
    ├── Services            # Business logic layer
    │   ├── Interfaces      # Service contracts
    │   └── Providers       # Business logic implementations (Facade pattern)
    ├── Utils               # Cross-cutting concerns
    │   ├── Mappers         # AutoMapper profiles
    │   ├── UnitOfWork      # Atomic transaction management
    │   └── Valids          # Specialized validation logic
    └── Properties          # JSON Launch settings and environment profiles

---

## Key Implementation Details

### Unit of Work & Repository Pattern
The project uses a centralized `UnitOfWork` to manage database transactions. This ensures that all operations within a single request (e.g., creating a post and linking categories) are atomic.


### Exception Handling
A Global Exception Middleware captures custom exceptions (like `ModelNotFoundException` or `ForbiddenException`) and maps them to appropriate HTTP Status Codes (404, 403, etc.), ensuring a consistent API response format.

### Domain Enums
All post-related states (Moderation, Visibility, Reading Level) are handled via Enums and mapped as Strings in the database through Fluent API for better readability in the TiDB/MySQL storage.

---

## Development Workflow

1. **Model Definition**: Add new entities in `Models`.
2. **Fluent Mapping**: Configure the entity in `AppDbContext` using Fluent API.
3. **Migrations**:
   dotnet ef migrations add Name_Of_Migration
4. **Service Layer**: Implement logic in `Services/Providers` and register it.
5. **Testing**: Add unit tests in the dedicated test project ensuring the mock logic for UoW and Mapper is consistent.

**Last Updated**: February 8, 2026