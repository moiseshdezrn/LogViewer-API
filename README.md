# LogViewer

.NET Web API project for log viewing and management, with a **LogDB** database and Code First Entity Framework Core.

---

## Prerequisites

| Requirement    | Version / Notes                                                                              |
| -------------- | -------------------------------------------------------------------------------------------- |
| **.NET SDK**   | 8.0 or later                                                                                 |
| **SQL Server** | SQL Server 2019+ or **SQL Server LocalDB** (included with Visual Studio / .NET SDK workload) |
| **Node.js**    | 18.x or 20.x LTS (only required if running or developing a frontend)                         |

- Install [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).
- For SQL Server: use an existing instance or install [LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb).

---

## Setup Instructions

### 1. Clone and restore

```bash
git clone <repository-url>
cd Logs-TechnicalExam
dotnet restore
```

### 2. Connection string configuration

Edit **`LogViewer.API/appsettings.json`** (and optionally `appsettings.Development.json`) and set the **LogDB** connection string for your environment.

Default (LocalDB):

```json
"ConnectionStrings": {
  "LogDB": "Server=(localdb)\\mssqllocaldb;Database=LogDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

For a named SQL Server instance:

```json
"LogDB": "Server=localhost\\SQLEXPRESS;Database=LogDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

For SQL Server with login:

```json
"LogDB": "Server=your-server;Database=LogDB;User Id=your-user;Password=your-password;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

### 3. Database creation with LocalDB (Code First)

#### Step 1: Verify LocalDB is installed

```powershell
sqllocaldb info
```

You should see `mssqllocaldb` or `MSSQLLocalDB` in the list. If not, install it from [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB is included).

#### Step 2: Start LocalDB instance (if needed)

```powershell
sqllocaldb start mssqllocaldb
```

#### Step 3: Install EF Core CLI tools (if not already installed)

```powershell
dotnet tool install --global dotnet-ef
```

Or update to the latest version:

```powershell
dotnet tool update --global dotnet-ef
```

#### Step 4: Apply migrations to create LogDB

From the repository root:

```powershell
dotnet ef database update --project LogViewer.Infrastructure --startup-project LogViewer.API
```

This command will:

- Create the **LogDB** database (if it doesn't exist)
- Create the **`[dbo].[Logs]`** table with all columns, indexes, and constraints
- Create the **`[dbo].[Users]`** table with unique indexes on Username and Email
- Insert a sample user for testing:
  - **Username:** `Sample username`
  - **Email:** `sample@test.com`
  - **Password:** `test` (stored as BCrypt hash)

#### Step 5: Verify database creation

```powershell
sqlcmd -S "(localdb)\mssqllocaldb" -d LogDB -Q "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'"
```

Expected output:

- `Logs`
- `Users`
- `__EFMigrationsHistory`

#### Creating new migrations (after model changes)

```powershell
dotnet ef migrations add YourMigrationName --project LogViewer.Infrastructure --startup-project Logs-TechnicalExam
dotnet ef database update --project LogViewer.Infrastructure --startup-project Logs-TechnicalExam
```

### 4. Data import (optional)

The project does not include a built-in data import. To load initial or sample data:

- Use SQL scripts against **LogDB** after the schema is created, or
- Add seed data in `LogDbContext.OnModelCreating` or a separate seeding step and run it on startup or via a one-off command.

---

## How to run the backend

From the repository root:

```bash
dotnet run --project Logs-TechnicalExam
```

Or from the API project folder:

```bash
cd UnosquareTechnicalExam
dotnet run
```

The API listens on the URLs shown in the console (e.g. `https://localhost:7xxx` and `http://localhost:5xxx`). Swagger UI is available at `/swagger` when running (typically in Development).

---

## How to run the frontend

This repository currently contains only the **backend** (LogViewer API). There is no frontend project in the solution.

If a frontend is added later (e.g. Angular or React in a subfolder):

```bash
cd path/to/frontend
npm ci
npm start
```

Use the same Node.js version as in **Prerequisites** (e.g. 18 or 20 LTS).

---

## How to run the tests

From the repository root:

```bash
dotnet test
```

To run with coverage (if your test project uses Coverlet):

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Tests are in **LogViewer.Tests** and use xUnit.
