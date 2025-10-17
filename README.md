# DemoCursorPagination

A .NET 9 ASP.NET Core minimal API demonstrating two pagination strategies: **Offset-based** and **Cursor-based** pagination using PostgreSQL and Entity Framework Core.

## 🚀 Features

- **Offset Pagination** (`/offset`): Traditional page-based pagination with page numbers
- **Cursor Pagination** (`/cursor`): Efficient cursor-based pagination for large datasets
- **PostgreSQL Integration**: Uses PostgreSQL 16 with proper indexing for optimal query performance
- **Entity Framework Core 9**: Leverages EF Core with Npgsql provider
- **Swagger/OpenAPI**: Interactive API documentation
- **Snake Case JSON**: RESTful API responses using snake_case naming convention

## 📋 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/get-started) (for PostgreSQL)
- [PostgreSQL 16](https://www.postgresql.org/) (or use Docker)

## 🗄️ Database Setup

### 1. Start PostgreSQL with Docker

```bash
docker run --name devhub-postgres \
  -e POSTGRES_PASSWORD=mysecretpassword \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_DB=devhub \
  -p 5432:5432 \
  -v devhub_pgdata:/var/lib/postgresql/data \
  -d postgres:16
```

### 2. Initialize Database Schema

Execute the SQL script to create the database schema:

```bash
# Connect to PostgreSQL and run the schema script
docker exec -i devhub-postgres psql -U postgres -d devhub < DemoCursorPagination/postgres.sql
```

### 3. (Optional) Seed Sample Data

To populate the database with sample data (1,000 users and 1,000,000 notes):

```bash
docker exec -i devhub-postgres psql -U postgres -d devhub < DemoCursorPagination/postgres_with_seed.sql
```

## 🏃 Running the Application

### 1. Configure Connection String

Update the connection string in `DemoCursorPagination/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Port=5432;Database=devhub;Username=postgres;Password=mysecretpassword"
  }
}
```

### 2. Build and Run

```bash
cd DemoCursorPagination
dotnet build
dotnet run
```

The application will start at `https://localhost:7001` (or as configured).

### 3. Access Swagger UI

Navigate to `https://localhost:7001/swagger` to explore the API interactively.

## 📡 API Endpoints

### GET `/offset` - Offset-based Pagination

Traditional pagination using page numbers and page sizes.

**Query Parameters:**
- `page` (int, default: 1): Page number (must be > 0)
- `pageSize` (int, default: 30): Number of items per page (1-100)

**Response:**
```json
{
  "items": [...],
  "page": 1,
  "page_size": 30,
  "total_count": 1000000,
  "total_pages": 33334,
  "has_previous_page": false,
  "has_next_page": true
}
```

**Example:**
```bash
GET /offset?page=1&pageSize=10
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
  "limit": 30,
  "has_more": true,
  "next_cursor": "eyJEYXRlIjoiMjAyNS0xMC0xNSIsIkxhc3RJZCI6ImZmYzlmOTMzLTM0YmMtNDIxOS1hNmFjLWZjNmU0NDk0MzEyOCJ9"
}
```

**Example:**
```bash
# First page
GET /cursor?limit=10

# Next page (using cursor from previous response)
GET /cursor?limit=10&cursor=eyJEYXRlIjoiMjAyNS0xMC0xNSIsIkxhc3RJZCI6ImZmYzlmOTMzLTM0YmMtNDIxOS1hNmFjLWZjNmU0NDk0MzEyOCJ9
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

- **Framework**: .NET 9 / ASP.NET Core Minimal APIs
- **Database**: PostgreSQL 16
- **ORM**: Entity Framework Core 9
- **Database Provider**: Npgsql.EntityFrameworkCore.PostgreSQL
- **API Documentation**: NSwag (Swagger/OpenAPI)
- **JSON Serialization**: System.Text.Json with snake_case naming

## 📁 Project Structure

```
DemoCursorPagination/
├── Data/
│   └── ApplicationDbContext.cs    # EF Core DbContext
├── Models/
│   ├── Users.cs                   # User entity
│   └── UserNotes.cs               # UserNote entity
├── Cursor.cs                      # Cursor encoding/decoding logic
├── Program.cs                     # Application entry point & API endpoints
├── postgres.sql                   # Database schema script
├── postgres_with_seed.sql         # Schema + seed data script
├── rest_client.http               # HTTP client test file
└── appsettings.json               # Configuration
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
GET https://localhost:7001/offset?page=1&pageSize=4

# Cursor pagination - first page
GET https://localhost:7001/cursor?limit=4

# Cursor pagination - next page
GET https://localhost:7001/cursor?limit=4&cursor=eyJEYXRlIjoiMjAyNS0xMC0xNSIsIkxhc3RJZCI6ImZmYzlmOTMzLTM0YmMtNDIxOS1hNmFjLWZjNmU0NDk0MzEyOCJ9
```

## 🔧 Entity Framework Core Scaffolding

To regenerate models from the database:

```bash
cd DemoCursorPagination
dotnet ef dbcontext scaffold \
  "Name=ConnectionStrings:Database" \
  Npgsql.EntityFrameworkCore.PostgreSQL \
  --context ApplicationDbContext \
  --output-dir Models \
  --context-dir Data \
  --namespace "DemoCursorPagination.Models" \
  --context-namespace "DemoCursorPagination.Data" \
  --schema identity --schema notes \
  --no-pluralize \
  --no-onconfiguring \
  --force
```

## 📝 License

This is a demonstration project for educational purposes.

## 👤 Author

[hoc081098](https://github.com/hoc081098)