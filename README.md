# ExpenseTracker

A REST API for tracking personal expenses, built with ASP.NET Core. Supports JWT authentication, VAT calculations, and spending summaries by category.

Live demo: expensetracker-production-743d.up.railway.app/swagger

## Tech Stack

- .NET 10, ASP.NET Core Web API
- Entity Framework Core with Npgsql (PostgreSQL)
- JWT Bearer authentication
- BCrypt.Net-Next for password hashing

## Setup

### Prerequisites

- .NET 10 SDK
- A PostgreSQL database (e.g. [Neon](https://neon.tech))

### Configuration

Add the following to `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your_postgres_connection_string"
  },
  "Jwt": {
    "Key": "your_secret_key",
    "Issuer": "your_issuer",
    "Audience": "your_audience",
    "ExpiryDays": 7
  }
}
```

### Run

```bash
dotnet run
```

Migrations are applied automatically on startup. Swagger UI is available at `/swagger`.

---

## API Reference

### Auth

#### Register
`POST /api/auth/register`

```json
{
  "username": "john",
  "email": "john@example.com",
  "password": "secret"

}
```

#### Login
`POST /api/auth/login`

```json
{
  "email": "john@example.com",
  "password": "secret"
}
```

Both return a JWT token on success:

```json
{
  "token": "eyJ...",
  "username": "john",
  "email": "john@example.com"
}
```

---

### Expenses

All expense endpoints require an `Authorization: Bearer <token>` header. Users can only access their own expenses.

Amounts are split into net, VAT, and gross. You provide the net amount and VAT rate — the API calculates the rest.

#### List all expenses
`GET /api/expenses`

#### Get a single expense
`GET /api/expenses/{id}`

#### Create an expense
`POST /api/expenses`

```json
{
  "title": "Groceries",
  "netAmount": 45.50,
  "vatRate": 20,
  "category": "Food",
  "date": "2026-03-27T00:00:00Z",
  "notes": "Weekly shop"
}
```

#### Update an expense
`PUT /api/expenses/{id}`

```json
{
  "title": "Groceries",
  "netAmount": 50.00,
  "vatRate": 20,
  "category": "Food",
  "date": "2026-03-27T00:00:00Z",
  "notes": "Weekly shop + extras"
}
```

#### Delete an expense
`DELETE /api/expenses/{id}`

Returns `204 No Content` on success.

---

### Summary

#### Spending summary by category
`GET /api/expenses/summary`

Optional query parameters:
- `from` — start date (e.g. `2026-01-01`)
- `to` — end date (e.g. `2026-03-31`)

Defaults to the date range of your earliest and latest expense.

Example response:

```json
{
  "from": "2026-01-01T00:00:00Z",
  "to": "2026-03-31T00:00:00Z",
  "totalNet": 175.00,
  "totalVat": 35.00,
  "totalGross": 210.00,
  "categories": [
    {
      "category": "Food",
      "totalNet": 125.00,
      "totalVat": 25.00,
      "totalGross": 150.00,
      "count": 5
    },
    {
      "category": "Transport",
      "totalNet": 50.00,
      "totalVat": 10.00,
      "totalGross": 60.00,
      "count": 3
    }
  ]
}
```
