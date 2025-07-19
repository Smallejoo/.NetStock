# ArmyStockApp

This repository contains a small ASP.NET 8 Web API that demonstrates how to manage a simple stock system with MongoDB. The solution includes:

- **Models** – Product and User entities
- **Services** – Application services that access MongoDB
- **Controllers** – API controllers exposing CRUD endpoints
- **settings** – `MongoDbSettings` and `appsettings.json` with database configuration

The actual project file resides in [`ArmyStockApp/`](ArmyStockApp/) and can be run from the command line.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download) installed
- MongoDB running locally or available from the connection string in `settings/appsettings.json`

## Running the API

Use the `dotnet run` command against the project file:

```bash
cd ArmyStockApp
dotnet run
```

By default the API listens on `http://localhost:5144` (adjust using the `--urls` option if needed). API endpoints are available under `/api/`.

## Building the solution

If you prefer using the solution file:

```bash
dotnet build STOCK.sln
```

This will restore packages and build the project.

---
Feel free to adapt the configuration or extend the controllers and models for your needs.
