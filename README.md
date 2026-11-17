# Library Book Management System (`A_PRN212`)

A desktop Library Management System built with **C# / .NET 8 WPF**, **Entity Framework Core**, and **SQL Server**, structured around **Clean Architecture** and **MVVM** with role-based workflows for **Admin**, **Librarian**, and **Reader**.

## Features

### Authentication & Account Security
- Sign in, sign up (defaults new accounts to `Reader`), and personal profile management.
- Password hashing with **SHA-256** (`PasswordHasher`) with constant-time hash comparison and automatic migration for legacy seed records.
- Password reset flow via **SMTP email OTP verification** and authenticated password updates.

### Role-Based Capabilities
- **Admin**: Full CRUD and multi-criteria search across Books, Authors, Genres, and Users; read-only overview of all borrow records.
- **Librarian**: Manage Books, Authors, Genres, and Readers; process borrow/return workflows, track overdue records, and settle fines.
- **Reader**: Browse and search books, authors, and genres; borrow available books (14-day loan period), view personal borrow history, pay overdue fines (`10,000 VND/day`), manage profile/password, and review library rules.

## Architecture & Project Structure

The solution (`A_PRN212.sln`) is split into 4 decoupled projects following Dependency Inversion:

```text
Book_Management/
├── A_PRN212.Domain/           # Entities, domain constants/enums, repository & UnitOfWork interfaces
├── A_PRN212.Application/      # Business services, DTOs, ServiceResult, UserSession, PasswordHasher
├── A_PRN212.Infrastructure/   # EF Core LibraryDbContext, Repositories, UnitOfWork, SMTP email, DI factory
├── A_PRN212/                  # WPF Views (XAML), ViewModels, MVVM helpers (RelayCommand, UiAnimation)
├── database.sql               # SQL Server schema and sample seed data
└── A_PRN212.sln
```

### Design Patterns Applied
- **Clean Architecture (4-Layer Separation)**: Domain and Application layers depend only on abstractions; Infrastructure implements persistence and SMTP integration.
- **MVVM (Model–View–ViewModel)**: Views bind cleanly to ViewModels via `INotifyPropertyChanged` (`ViewModelBase`) and `ICommand` (`RelayCommand`) without database logic in code-behind.
- **Repository & Unit of Work**: Generic and entity-specific repositories coordinated through `IUnitOfWork` and `LibraryDbContext`.
- **Singleton & Factory**: Thread-safe `Lazy<T>` singletons for `UserSession`, `DatabaseWorkManager`, `ServiceProviderFactory`, and `NavigationHelper` for role-based window routing.

## Tech Stack
- **Framework**: .NET 8 (`net8.0` / `net8.0-windows`), WPF
- **ORM & Database**: Entity Framework Core `8.0.13`, Microsoft SQL Server
- **Configuration & Email**: `Microsoft.Extensions.Configuration.Json`, `System.Net.Mail` (SMTP)

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (Windows)
- SQL Server (local instance `.` or SQL Server Express)
- Visual Studio 2022 (optional, for IDE development)

### 1. Set Up the Database
Run `database.sql` in SQL Server Management Studio (SSMS) or via `sqlcmd`:

```powershell
sqlcmd -S . -E -C -i database.sql
```

This creates the `A_PRN212` database, tables, constraints, and sample data (authors, genres, books, library rules, cards, and users).

### 2. Configure `A_PRN212/appsettings.json`
Update the connection string and SMTP settings for password-reset OTP emails:

```json
{
  "ConnectionStrings": {
    "MyCnn": "Data Source=.;Initial Catalog=A_PRN212;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true"
  },
  "GmailOptions": {
    "Hosts": "smtp.gmail.com",
    "Port": 587,
    "Email": "your-email@gmail.com",
    "Password": "your-gmail-app-password"
  }
}
```

### 3. Build & Run

```powershell
dotnet build A_PRN212.sln
dotnet run --project A_PRN212/A_PRN212.csproj
```

## Demo Accounts

Seeded in `database.sql` (stored as SHA-256 hashes):

| Role | Username | Password |
|---|---|---|
| Admin | `dtd` | `123` |
| Admin | `admin2` | `Admin@123` |
| Librarian | `librarian1` | `Lib@1234` |
| Reader | `reader1` | `Reader@123` |
