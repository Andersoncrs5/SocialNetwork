# Social Network - Distributed Microservices Platform

A highly scalable social network ecosystem built with .NET 10, leveraging the CQRS (Command Query Responsibility Segregation) pattern to decouple high-load write operations from performant read projections.

## Architecture Overview

The system is split into two main operational pillars and a set of supporting microservices:

* **SocialNetwork.Write.API**: Handles all domain commands, business logic validation, and primary data persistence.
* **SocialNetwork.Read.API**: Optimized for low-latency queries, utilizing read models and high-speed caching.
* **Core Services**:
    * EmailSender: Background processing for transactional emails.
    * Metrics: Calculation engine for user engagement and post rankings.
    * Notifications: Real-time alerts and push notification management.

## Tech Stack

* **Runtime**: .NET 10
* **Primary Database**: TiDB (Distributed SQL / MySQL Compatible)
* **High-Performance Cache**: Dragonfly (Redis-compatible multi-threaded store)
* **Document Store**: MongoDB (For read projections and logging)
* **Cloud Storage**: AWS S3 (Media, featured images, and profile assets)
* **Infrastructure**: Docker & Docker Compose for orchestration

---

## Project Structure
    
    .
    ├── src
    │   ├── BuildingBlocks          # Shared abstractions and cross-cutting concerns
    │   │   ├── SocialNetwork.Contracts       # Global DTOs, Events, and Interfaces
    │   │   └── SocialNetwork.Infrastructure  # DB Providers, Auth, and Logging
    │   ├── Services                # Specialized Background Microservices
    │   │   ├── EmailSender
    │   │   ├── Metrics
    │   │   └── Notifications
    │   └── WebApps                 # Main Entry Points (CQRS Split)
    │       ├── SocialNetwork.Read.API
    │       └── SocialNetwork.Write.API
    ├── tests                       # Quality Assurance Suites
    │   ├── SocialNetwork.Tests               # Unit Tests (Logic & Services)
    │   └── SocialNetwork.Write.IntegrationTests # End-to-End API Integration
    └── docker-compose.yaml         # Local infrastructure orchestration

---

## Getting Started

### Prerequisites
- Docker Desktop
- .NET 10 SDK

## Testing Policy

Reliability is ensured through a rigorous testing strategy:

* **Unit Tests**: Located in tests/SocialNetwork.Tests. Focuses on Business Logic, Mocks, and Service behavior.
* **Integration Tests**: Located in tests/SocialNetwork.Write.IntegrationTests. Validates the flow between API, Repository, and the TiDB database.

---

## Roadmap
- [ ] Integration with AWS S3 for post media uploads.
- [ ] Advanced Cache-Aside strategy for the Read API.

**Last Updated**: February 8, 2026