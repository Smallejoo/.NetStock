# Stock management API

A C# / ASP.NET Core 8 portfolio project for maintaining a shared inventory: register an account, sign in, and create, view, edit or delete products stored in MongoDB. A small browser demo is included.

## What this demonstrates

- Controllers for HTTP requests, services for database operations, and MongoDB-backed models.
- BCrypt password hashing; POST login with protected bearer tokens; responses that omit password hashes.
- Authenticated inventory operations and self-service account changes.
- Input validation and automated HTTP integration tests.

This is a learning/demo application, not a deployed production inventory system. All registered accounts share inventory access; it does not implement administrator roles or per-organization stock.

## Architecture

```text
Browser demo / HTTP client
          | JSON + Authorization: Bearer <token>
ASP.NET Core controllers
          | validated requests
UserService / ProductService
          | MongoDB driver
MongoDB (Users, Products)
```

The canonical solution is `STOCK.sln`. Application code lives only in `ArmyStockApp/`; tests live in `ArmyStockApp.Tests/`.

## Run locally

Requirements: .NET 8 SDK and a local MongoDB server. Docker is optional; it can provide MongoDB:

```sh
docker run --name stock-mongo -p 127.0.0.1:27017:27017 -d mongo:7
dotnet restore STOCK.sln
dotnet run --project ArmyStockApp --launch-profile http
```

Open http://localhost:5144 for the demo or http://localhost:5144/swagger for development API documentation. Register with a unique username, email and a password of 12–64 characters, then sign in. The demo keeps its token in memory; reloading signs you out locally.

Default database: `ArmyStock` at `mongodb://localhost:27017`. For another server, set `MongoDB__ConnectionString` and `MongoDB__DatabaseName` in your environment. Never commit real credentials. Use synthetic sample data only.

## API walkthrough

1. `POST /api/Users` with `{ "userName": "demo", "email": "demo@example.com", "password": "LongDemoPassword123" }`.
2. `POST /api/Users/login` with `{ "userName": "demo", "password": "LongDemoPassword123" }`.
3. Copy `accessToken` from the response into the `Authorization: Bearer <token>` header for protected requests.
4. `POST /api/Products` with `{ "name": "Sensor", "description": "Demo sensor", "stock": 5, "price": 12.50 }`.
5. `GET /api/Products`, then `PUT` or `DELETE /api/Products/{id}` to manage a product.

| Endpoint | Purpose | Access |
|---|---|---|
| POST /api/Users | Register | Public |
| POST /api/Users/login | Sign in | Public |
| GET /api/Users/me | Current account | Bearer token |
| PATCH /api/Users/changeEmail | Change own email; body: password, newEmail | Bearer token + password |
| DELETE /api/Users/me | Delete own account; body: password | Bearer token + password |
| GET/POST /api/Products | List/create products | Bearer token |
| GET/PUT/DELETE /api/Products/{id} | Read/update/delete product | Bearer token |

Authentication uses ASP.NET Core's built-in opaque bearer tokens, not JWT. Existing unauthenticated clients must now sign in. The obsolete GET login and arbitrary-email deletion routes have been removed.

## Build and test

```sh
dotnet build STOCK.sln
dotnet test STOCK.sln
```

The HTTP integration tests start the actual application with an in-memory user-service substitute. They check registration validation, login, token rejection, safe responses, account identity and anonymous-access restrictions. They do not require MongoDB and do not verify database persistence. Manually verify create/read/update/delete against MongoDB before a release. GitHub Actions runs build and tests.

## Limitations and next improvements

- Add database-backed tests, unique indexes and concurrency-safe account creation.
- Add rate limiting, email verification, role-based permissions and account lockout before public deployment.
- Deleting an account does not revoke already-issued access tokens immediately. There is no refresh endpoint; sign in again after expiry.
- Production hosting needs HTTPS, protected/persistent Data Protection keys and a deliberate host configuration.
- Extend the demo with edit/delete controls and add screenshots after verifying it with a running database.
