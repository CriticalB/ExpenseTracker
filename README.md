# ExpenseTracker

A REST API for tracking personal expenses, built with ASP.NET Core.

## Tech Stack

- .NET 10, ASP.NET Core Web API
- Entity Framework Core with Npgsql (PostgreSQL)
- JWT Bearer authentication
- BCrypt.Net-Next for password hashing
- Neon hosted PostgreSQL

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
dotnet ef database update
dotnet run
```

Swagger UI is available at `https://localhost:{port}/swagger` in development.

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

All expense endpoints require a `Authorization: Bearer <token>` header. Users can only access their own expenses.

#### List all expenses
`GET /api/expenses`

#### Get a single expense
`GET /api/expenses/{id}`

#### Create an expense
`POST /api/expenses`

```json
{
  "title": "Groceries",
  "amount": 45.50,
  "category": "Food",
  "date": "2026-03-26T00:00:00Z",
  "notes": "Weekly shop"
}
```

#### Update an expense
`PUT /api/expenses/{id}`

```json
{
  "title": "Groceries",
  "amount": 50.00,
  "category": "Food",
  "date": "2026-03-26T00:00:00Z",
  "notes": "Weekly shop + extras"
}
```

#### Delete an expense
`DELETE /api/expenses/{id}`

#### Summary by category
`GET /api/expenses/summary`

Optional query parameters:
- `from` — start date (e.g. `2026-01-01`)
- `to` — end date (e.g. `2026-03-31`)

Example response:

```json
{
  "from": "2026-01-01T00:00:00Z",
  "to": "2026-03-31T00:00:00Z",
  "totalAmount": 210.00,
  "categories": [
    { "category": "Food", "totalAmount": 150.00, "count": 5 },
    { "category": "Transport", "totalAmount": 60.00, "count": 3 }
  ]
}
```
