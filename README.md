# BeSpoked Bikes — Sales Tracking Application

A full-stack sales tracking app for BeSpoked, a high-end bicycle shop. Tracks salesperson commissions, product inventory, customer records, and quarterly bonuses.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | .NET 8 Web API (C#) |
| ORM | Entity Framework Core 8 (Code-First) |
| Database | SQL Server (LocalDB for dev) |
| Frontend | React 18 + React Router 6 |
| Architecture | 3-tier: Core → Infrastructure → API |

---

## Project Structure

```
BeSpoked/
├── backend/
│   ├── BeSpoked.Core/            # Entities, interfaces (no dependencies)
│   │   ├── Entities/             # Product, Salesperson, Customer, Sale, Discount
│   │   └── Interfaces/           # IRepository<T>, ISaleRepository
│   ├── BeSpoked.Infrastructure/  # EF Core, repositories
│   │   ├── Data/AppDbContext.cs  # DbContext with fluent config + seed data
│   │   └── Repositories/        # Generic + Sale-specific repositories
│   └── BeSpoked.API/             # ASP.NET Web API controllers
│       ├── Controllers/          # Salespersons, Products, Customers, Sales
│       └── Program.cs            # DI, CORS, EF migration on startup
└── frontend/
    └── src/
        ├── services/api.js       # All API calls in one place
        ├── components/
        │   ├── Salespersons/     # List + edit modal
        │   ├── Products/         # List + edit modal
        │   ├── Customers/        # List
        │   ├── Sales/            # List with date filter + Create form
        │   └── Reports/          # Quarterly commission report
        └── App.js                # Router + nav
```

---

## Setup & Run

### Prerequisites
- .NET 8 SDK
- SQL Server (or LocalDB — ships with Visual Studio)
- Node.js 18+

### Backend

```bash
cd backend

# Restore packages
dotnet restore

# Create the database + run migrations + seed data
# (happens automatically on first run in Development)
cd BeSpoked.API
dotnet run
# API runs on https://localhost:5001 / http://localhost:5000
```

If you prefer to run migrations manually:
```bash
cd backend
dotnet ef migrations add InitialCreate --project BeSpoked.Infrastructure --startup-project BeSpoked.API
dotnet ef database update --project BeSpoked.Infrastructure --startup-project BeSpoked.API
```

### Frontend

```bash
cd frontend
npm install
npm start
# React dev server on http://localhost:3000
# Proxies /api/* to http://localhost:5000 via package.json proxy
```

---

## Key Design Decisions

### 1. 3-Tier Architecture
- **Core** — pure C# entities and interfaces, no EF or HTTP dependencies
- **Infrastructure** — EF Core implementation; only this layer knows about SQL Server
- **API** — thin controllers; business logic stays in repositories

### 2. Generic Repository + Specialization
`IRepository<T>` handles standard CRUD. `ISaleRepository` extends it with date-range and quarterly filtering, keeping the API controllers simple.

### 3. No Duplicate Constraint
Enforced at the database level via EF unique indexes (not just application logic):
- Products: unique on `(Name, Manufacturer)`
- Salespersons: unique on `(FirstName, LastName, Phone)`

### 4. Price Snapshot on Sale
When a sale is created, the current `SalePrice` and `CommissionPercentage` are copied onto the `Sale` record. This means historical sales are unaffected if product prices change later.

### 5. Automatic Discount Application
When creating a sale, the API queries the `Discounts` table for any active discount on that product + date. The discount percentage is stored on the sale — no recalculation needed later.

### 6. Commission Computation
`FinalPrice` and `Commission` are computed C# properties on the `Sale` entity, marked as `[NotMapped]` via EF's `Ignore()`. Calculated from stored values — no magic numbers in the DB.

### 7. Quarterly Bonus
The top-earning salesperson per quarter receives a 10% bonus on their total commission. Calculated server-side in the report endpoint and flagged in the JSON response.

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/salespersons` | List all salespersons |
| PUT | `/api/salespersons/{id}` | Update a salesperson |
| GET | `/api/products` | List all products |
| PUT | `/api/products/{id}` | Update a product |
| GET | `/api/customers` | List all customers |
| GET | `/api/sales?from=&to=` | List sales, optional date filter |
| POST | `/api/sales` | Create a sale |
| GET | `/api/sales/report?year=&quarter=` | Quarterly commission report |

Swagger UI available at `/swagger` in Development mode.
