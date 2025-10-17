# DemoCursorPagination

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

```bash
docker run --name devhub-postgres \
  -e POSTGRES_PASSWORD=mysecretpassword \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_DB=devhub \
  -p 5432:5432 \
  -v devhub_pgdata:/var/lib/postgresql/data \
  -d postgres:16
```