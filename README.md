# BeSpoked Bikes — Sales Tracking Application

A full-stack sales tracking app for BeSpoked, a high-end bicycle shop. Tracks salesperson commissions, product inventory, customer records, discounts, and quarterly bonuses.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | .NET 8 Web API (C#) |
| ORM | Entity Framework Core 8 (Code-First) |
| Database | SQL Server (Docker/local) · Azure SQL Database (production) |
| Frontend | React 18 + TypeScript + React Router 6 |
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
│   ├── BeSpoked.API/             # ASP.NET Web API controllers
│   │   ├── Controllers/          # Salespersons, Products, Customers, Sales, Discounts
│   │   ├── DTOs/                 # Response record types
│   │   ├── Settings/             # QuarterlyBonusSettings (appsettings.json binding)
│   │   └── Program.cs            # DI, CORS, global exception handler, EF migration on startup
│   └── BeSpoked.Tests/           # xUnit test suite
│       ├── Controllers/          # Controller unit tests (Moq)
│       ├── Repositories/         # Repository integration tests (EF InMemory)
│       └── Domain/               # BonusCalculator + Sale computed property tests
└── frontend/
    └── src/
        ├── types.ts              # Shared TypeScript interfaces matching backend DTOs
        ├── services/api.ts       # All API calls in one place
        ├── hooks/
        │   ├── useFetch.ts       # Generic data fetching hook
        │   └── useEditState.ts   # Create/edit form state hook
        ├── components/
        │   ├── common/           # Modal, FormField (shared UI primitives)
        │   ├── Salespersons/     # List + create/edit modal
        │   ├── Products/         # List + edit modal
        │   ├── Customers/        # List + create modal
        │   ├── Discounts/        # List + create/edit/delete modal
        │   ├── Sales/            # List with date filter + Create form
        │   └── Reports/          # Quarterly commission report
        └── App.tsx               # Router + nav
```

---

## Setup & Run

### Option 1 — Docker (recommended)

Requires [Docker Desktop](https://www.docker.com/products/docker-desktop/).

```bash
docker compose up --build
```

That's it. Docker will:
1. Start SQL Server
2. Build and run the .NET API (runs migrations + seeds data automatically)
3. Build the React app and serve it via nginx

| Service  | URL                        |
|----------|----------------------------|
| Frontend | http://localhost:3000      |
| API      | http://localhost:5001      |
| Swagger  | http://localhost:5001/swagger |

To stop: `docker compose down`
To wipe the database too: `docker compose down -v`

---

### Option 2 — Run locally

**Prerequisites:** .NET 8 SDK, Node.js 18+, Docker Desktop

```bash
# Step 1 — Start SQL Server
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=BeSpoked123!" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest

# Step 2 — Start the API (new terminal)
cd backend/BeSpoked.API
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Step 3 — Start the frontend (new terminal)
cd frontend
npm install
npm start
```

Open http://localhost:3000. Migrations and seed data run automatically on first startup.

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

### 7. Configurable Quarterly Bonus
Bonus rules are driven by `appsettings.json` — no hardcoded values in the controller:

```json
"QuarterlyBonus": {
  "TopN": 1,
  "BonusPercentage": 10.0
}
```

- `TopN` — how many top earners receive the bonus
- `BonusPercentage` — bonus as a percentage of the earner's total commission

### 8. Terminated Salesperson Validation
Rejected at both layers: the frontend filters terminated salespersons out of the dropdown, and the API returns `400 Bad Request` if a terminated salesperson's ID is submitted directly.

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/salespersons` | List all salespersons |
| POST | `/api/salespersons` | Create a salesperson |
| PUT | `/api/salespersons/{id}` | Update a salesperson |
| GET | `/api/products` | List all products |
| POST | `/api/products` | Create a product |
| PUT | `/api/products/{id}` | Update a product |
| GET | `/api/customers` | List all customers |
| POST | `/api/customers` | Create a customer |
| GET | `/api/discounts` | List all discounts |
| POST | `/api/discounts` | Create a discount |
| PUT | `/api/discounts/{id}` | Update a discount |
| DELETE | `/api/discounts/{id}` | Delete a discount |
| GET | `/api/sales?from=&to=` | List sales, optional date filter |
| POST | `/api/sales` | Create a sale |
| GET | `/api/sales/report?year=&quarter=` | Quarterly commission report |

Swagger UI available at `/swagger` in Development mode.
