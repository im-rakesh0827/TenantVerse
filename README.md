# 🏢 TenantVerse

> **Enterprise Property Rental & Tenant Management System built with ASP.NET Core 8, Blazor Web App, Dapper, SQL Server, and Clean Architecture.**

TenantVerse is a modern property management platform designed for landlords and property managers to efficiently manage rental properties, flats, tenants, invoices, and payments from a single application.

The project follows **Clean Architecture**, uses **Dapper with Stored Procedures**, and is built with **ASP.NET Core 8** and **Blazor Web App (.NET 8)** to demonstrate enterprise-grade software development practices.

---

# 🚀 Features

## Dashboard
- Property Statistics
- Total Flats
- Active Tenants
- Pending Payments
- Recent Properties

## Property Management
- Create Property
- Update Property
- Delete Property
- View Property Details
- Search
- Pagination
- Sorting
- Validation

## Flat Management *(In Progress)*
- Manage Flats
- Flat Availability
- Occupancy Status

## Tenant Management *(Planned)*
- Tenant Registration
- Tenant Profile
- Rental History

## Invoice Management *(Planned)*
- Monthly Rent Invoice
- Electricity Bill Calculation
- Maintenance Charges
- Invoice PDF Generation

## Payment Management *(Planned)*
- Rent Collection
- Payment History
- Outstanding Dues

## Authentication *(Planned)*
- JWT Authentication
- Role-Based Authorization

---

# 🛠 Technology Stack

### Backend

- ASP.NET Core 8 Web API
- C#
- Dapper
- SQL Server
- REST APIs
- Clean Architecture

### Frontend

- Blazor Web App (.NET 8)
- MudBlazor
- C#
- Razor Components

### Database

- SQL Server
- Stored Procedures
- Views
- Functions

### Development Tools

- Visual Studio Code
- Git
- GitHub
- Postman
- Swagger

---

# 📂 Project Structure

```
TenantVerse
│
├── TenantVerse.API
├── TenantVerse.Application
├── TenantVerse.Infrastructure
├── TenantVerse.Shared
├── TenantVerse.UI
│
└── Database
    ├── Tables
    ├── StoredProcedures
    ├── Views
    ├── Functions
    └── SeedData
```

---

# 🏗 Architecture

```
Blazor UI
      │
      ▼
ASP.NET Core Web API
      │
      ▼
Application Layer
      │
      ▼
Infrastructure Layer
      │
      ▼
SQL Server
```

---

# 📊 Current Modules

| Module | Status |
|---------|--------|
| Dashboard | ✅ Completed |
| Property Management | ✅ Completed |
| Flat Management | 🚧 In Progress |
| Tenant Management | 📅 Planned |
| Invoice Management | 📅 Planned |
| Payment Management | 📅 Planned |
| Authentication | 📅 Planned |

---

# 📸 Screenshots

Coming Soon

- Dashboard
- Property List
- Create Property
- Edit Property
- View Property
- Flat Management
- Invoice Management

---

# ⚙️ Getting Started

## Clone Repository

```bash
git clone https://github.com/your-username/TenantVerse.git
```

---

## Restore Packages

```bash
dotnet restore
```

---

## Configure Database

Update your connection string in:

```
appsettings.Development.json
```

---

## Run SQL Scripts

Execute scripts from the **Database** folder.

---

## Run API

```bash
cd TenantVerse.API

dotnet run
```

---

## Run UI

```bash
cd TenantVerse.UI

dotnet run
```

---

# 📚 Database

The Database folder contains:

- Database Creation Script
- Table Scripts
- Stored Procedures
- Views
- Functions
- Seed Data

---

# 🔒 Security

Sensitive information such as:

- Database Credentials
- API Keys
- JWT Secrets
- Azure Credentials

are excluded from source control.

---

# 🎯 Roadmap

- Flat Module
- Tenant Module
- Invoice Module
- Payment Module
- Dashboard Analytics
- Email Notifications
- Redis Caching
- SignalR Real-time Updates
- Azure Deployment
- Docker Support
- Unit Testing
- Integration Testing

---

# 🤝 Contributing

Contributions, suggestions, and feedback are always welcome.

Feel free to fork the repository and create a pull request.

---

# 📄 License

This project is intended for learning, portfolio, and demonstration purposes.

---

# 👨‍💻 Author

**Rakesh Kumar**

.NET Full Stack Developer

- LinkedIn: https://linkedin.com/in/im-rakesh0827/
- GitHub: https://github.com/im-rakesh0827

---

⭐ If you found this project helpful, consider giving it a star.