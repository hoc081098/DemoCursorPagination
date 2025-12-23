# Demo Cursor Pagination

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-18-336791?logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Build](https://github.com/yourusername/DemoCursorPagination/workflows/.NET%20Build%20and%20Test/badge.svg)](https://github.com/yourusername/DemoCursorPagination/actions)

A .NET 10 ASP.NET Core minimal API demonstrating two pagination strategies: **Offset-based** and **Cursor-based** pagination using PostgreSQL and Entity Framework Core.

This repository serves as a practical example and learning resource for implementing efficient pagination patterns in modern .NET applications.

> **💡 Tip**: This project demonstrates production-ready pagination patterns that scale efficiently with large datasets.

## 🚀 Features

- **Offset Pagination** (`/offset`): Traditional page-based pagination with page numbers and total counts
- **Cursor Pagination** (`/cursor`): Efficient cursor-based pagination for large datasets and real-time data
- **PostgreSQL Integration**: Uses PostgreSQL 18 with proper indexing for optimal query performance
- **Entity Framework Core 10**: Leverages EF Core with Npgsql provider
- **Clean Architecture**: Endpoints organized with dependency injection and separation of concerns
- **Typed Results**: Strongly-typed HTTP results using `TypedResults` for better API contracts
- **Swagger/OpenAPI**: Interactive API documentation with NSwag
- **Snake Case JSON**: RESTful API responses using snake_case naming convention
- **Docker Compose**: Easy setup with PostgreSQL database initialization and seeding
- **Database Seeding**: Automatic schema creation and sample data insertion on first run

## 📋 Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/get-started) (for running PostgreSQL)

## 🗄️ Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/DemoCursorPagination.git
cd DemoCursorPagination
```

### 2. Start PostgreSQL with Docker Compose

The Docker Compose setup will automatically:
- Start PostgreSQL 18 container
- Create the database schema
- Seed sample data

```bash
docker compose up -d
```

**Note**: The SQL initialization scripts (`postgres.sql` and `postgres_with_seed.sql`) only run when the database is first created. To re-run them:

```bash
docker compose down -v  # Remove containers and volumes
docker compose up -d    # Start fresh
```

### 3. Run the Application

```bash
cd DemoCursorPagination
dotnet restore
dotnet run
```

The application will start at `https://localhost:7001`.

### 4. Explore the API

Navigate to `https://localhost:7001/swagger` to explore the API interactively using Swagger UI.

## 📡 API Endpoints

### GET `/offset` - Offset-based Pagination

Traditional pagination using page numbers and page sizes.

**Query Parameters:**
- `page` (int, default: 1): Page number (must be > 0)
- `page_size` (int, default: 30): Number of items per page (1-100)

**Response:**
```json
{
  "items": [...],
  "metadata": {
    "page": 1,
    "page_size": 30,
    "total_count": 1000000,
    "total_pages": 33334,
    "has_previous_page": false,
    "has_next_page": true
  }
}
```

**Example:**
```bash
# First page
GET /offset?page=1&page_size=10

# Second page
GET /offset?page=2&page_size=10
```

### GET `/cursor` - Cursor-based Pagination

Efficient pagination using cursors, ideal for large datasets and real-time data.

**Query Parameters:**
- `cursor` (string, optional): Base64-encoded cursor for the next page
- `limit` (int, default: 30): Number of items to fetch (1-100)

**Response:**
```json
{
  "items": [...],
  "metadata": {
    "limit": 30,
    "has_more": true,
    "next_cursor": "eyJEYXRlIjoiMjAyNS0xMC0xNSIsIkxhc3RJZCI6ImZmYzlmOTMzLTM0YmMtNDIxOS1hNmFjLWZjNmU0NDk0MzEyOCIsIlZlcnNpb24iOjF9"
  }
}
```

**Example:**
```bash
# First page
GET /cursor?limit=10

# Next page (using cursor from previous response)
GET /cursor?limit=10&cursor=eyJEYXRlIjoiMjAyNS0xMC0xNSIsIkxhc3RJZCI6ImZmYzlmOTMzLTM0YmMtNDIxOS1hNmFjLWZjNmU0NDk0MzEyOCIsIlZlcnNpb24iOjF9
```

## 🔍 Pagination Comparison

### Offset Pagination
**Pros:**
- Simple to understand and implement
- Easy to jump to specific pages
- Total count available

**Cons:**
- Performance degrades with large offsets (OFFSET 10000 is slow)
- Inconsistent results if data changes between requests
- Requires counting total records

### Cursor Pagination
**Pros:**
- Consistent performance regardless of position in dataset
- Handles real-time data changes gracefully
- No expensive COUNT queries
- More efficient for large datasets

**Cons:**
- Cannot jump to arbitrary pages
- No total count information
- Slightly more complex implementation

## 🛠️ Technology Stack

- **Framework**: .NET 10 / ASP.NET Core Minimal APIs
- **Database**: PostgreSQL 18 Alpine
- **ORM**: Entity Framework Core 10
- **Database Provider**: Npgsql.EntityFrameworkCore.PostgreSQL
- **API Documentation**: NSwag (Swagger/OpenAPI)
- **JSON Serialization**: System.Text.Json with snake_case naming
- **Containerization**: Docker & Docker Compose

## 📁 Project Structure

```
DemoCursorPagination/
├── Contracts/
│   ├── Cursor.cs                  # Cursor encoding/decoding logic
│   └── NoteResponse.cs            # Note response DTO
├── Data/
│   └── ApplicationDbContext.cs    # EF Core DbContext
├── Models/
│   ├── Users.cs                   # User entity
│   └── UserNotes.cs               # UserNote entity
├── CursorEndpoint.cs              # Cursor pagination endpoint
├── OffsetEndpoint.cs              # Offset pagination endpoint
├── Program.cs                     # Application entry point & API configuration
├── postgres.sql                   # Database schema script
├── postgres_with_seed.sql         # Schema + seed data script
├── rest_client.http               # HTTP client test file
├── appsettings.json               # Configuration
└── appsettings.Development.json   # Development configuration
```

## 📊 Database Schema

The application uses two schemas:

### `identity.users`
- `id` (UUID): Primary key
- `name` (VARCHAR(20)): User name

### `notes.user_notes`
- `id` (UUID): Primary key
- `user_id` (UUID): Foreign key to users
- `note` (VARCHAR(500)): Note content
- `note_date` (DATE): Date of the note

**Indexes:**
- `idx_user_notes_note_date_id` (note_date DESC, id DESC) - Optimizes cursor pagination queries
- `idx_user_notes_user_id` - Optimizes user lookups

## 🧪 Testing the API

Use the provided `rest_client.http` file with VS Code REST Client extension or any HTTP client:

```http
# Offset pagination
GET https://localhost:7001/offset?page=1&page_size=4

# Cursor pagination - first page
GET https://localhost:7001/cursor?limit=4

# Cursor pagination - next page (use cursor from previous response)
GET https://localhost:7001/cursor?limit=4&cursor=eyJEYXRlIjoiMjAyNS0xMi0yMyIsIkxhc3RJZCI6IjAxOWI0YTBjLTU1MTQtNzJjOS1hYmQyLWNmNWMwMWQ2NjZmYyIsIlZlcnNpb24iOjF9
```

## 🔧 Configuration

### Connection String

The application reads the connection string from `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Port=5434;Database=DemoCursorPagination;Username=postgres;Password=postgres"
  }
}
```

**Note**: The PostgreSQL container exposes port `5434` on the host (mapped to `5432` in the container).

### Docker Compose Configuration

The `compose.yaml` file automatically:
- Runs PostgreSQL 18 Alpine
- Creates the database `DemoCursorPagination`
- Executes `postgres.sql` (schema) and `postgres_with_seed.sql` (seeding) on first initialization
- Persists data in a Docker volume

## 🎓 Learning Resources

This project demonstrates:

1. **Minimal APIs in .NET 10** - Lightweight API development without controllers
2. **Dependency Injection** - Scoped services for endpoint handlers
3. **Entity Framework Core** - Database access with proper indexing
4. **Typed Results** - Using `TypedResults` for strongly-typed HTTP responses
5. **Cursor-based Pagination** - Implementation using composite indexes
6. **Docker Integration** - Database initialization with Docker Compose
7. **Clean Architecture** - Separation of concerns with dedicated endpoint classes

## 🤝 Contributing

Contributions, issues, and feature requests are welcome! Feel free to check the [issues page](../../issues).

## 📝 License

This project is [MIT](LICENSE) licensed.

## 👤 Author

**Hoc Nguyen**
- GitHub: [@hoc081098](https://github.com/hoc081098)

---

⭐ Star this repository if you find it helpful!
