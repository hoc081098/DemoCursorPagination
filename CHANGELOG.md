# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0] - 2025-12-23

### Added
- Initial release of Demo Cursor Pagination
- Offset-based pagination endpoint (`/offset`)
- Cursor-based pagination endpoint (`/cursor`)
- PostgreSQL 18 database integration with Docker Compose
- Entity Framework Core 10 with Npgsql provider
- Swagger/OpenAPI documentation with NSwag
- Snake case JSON serialization
- Clean architecture with dedicated endpoint classes
- Database initialization scripts with automatic seeding
- Comprehensive README with usage examples
- MIT License
- Contributing guidelines
- GitHub Actions CI/CD workflows
- Issue templates for bugs and feature requests
- Security policy documentation
- EditorConfig for consistent code style

### Features
- **OffsetEndpoint**: Traditional page-based pagination with total counts
- **CursorEndpoint**: Efficient cursor-based pagination for large datasets
- **Typed Results**: Strongly-typed HTTP responses using TypedResults
- **Structured Logging**: Proper logging with ILogger
- **Docker Support**: Easy setup with docker-compose
- **API Documentation**: Interactive Swagger UI

### Database
- UUID v7 support for primary keys
- Composite indexes for optimal query performance
- Two schemas: `identity` and `notes`
- Foreign key constraints with cascade delete
- Sample seed data for testing

[Unreleased]: https://github.com/yourusername/DemoCursorPagination/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/yourusername/DemoCursorPagination/releases/tag/v1.0.0

