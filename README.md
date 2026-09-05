# ExpenseTracker.Api

ASP.NET Core Web API backend for the [Daily Expense Tracker](https://github.com/michelleconchina/expense-tracker) learning project. Paired with a React Native + TypeScript frontend, built one phase at a time.

## Stack

- ASP.NET Core Web API (.NET 10)
- Swashbuckle (Swagger UI)
- EF Core + SQLite

## Running locally

Requires the [.NET SDK](https://dotnet.microsoft.com/download).

```bash
dotnet run
```

The API starts on the URL printed in the console (see `Properties/launchSettings.json`). With the app running in development mode, Swagger UI is available at `/swagger`.

## Endpoints

| Method | Route              | Description                  |
|--------|--------------------|-------------------------------|
| GET    | `/api/ping`         | Health check, returns `pong` |

More endpoints (CRUD for expenses, daily summary) land in later phases — see the project plan.

## Project plan

This repo is built alongside a phased checklist (setup → backend foundations → data layer → CRUD API → frontend → polish). Ask in the paired frontend repo or check your saved plan artifact for the full breakdown.
