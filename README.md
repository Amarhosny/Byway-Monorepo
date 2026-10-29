# Byway — Online Learning Platform

Byway is a full-stack online learning platform built as a learning and portfolio project. It provides course browsing, authentication, shopping cart and checkout flows, with separate student and admin experiences.

## Tech Stack

### Backend
- C#
- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- PostgreSQL
- JWT authentication and role-based authorization
- Onion Architecture

### Frontend
- React
- JavaScript
- Vite
- React Router

## Repository Structure

```text
Byway-Monorepo/
├── backend/
│   └── Byway/
│       └── src/
│           ├── Byway.API/
│           ├── Byway.Application/
│           ├── Byway.Domain/
│           └── Byway.Infrastructure/
└── frontend/
    └── byway-react/
```

## Prerequisites

- .NET 8 SDK
- Node.js and npm
- PostgreSQL

## Run Locally

### 1. Configure the Backend

From the repository root:

```powershell
cd backend/Byway
dotnet restore
dotnet build
```

Configure the database connection string and JWT signing key using .NET User Secrets. Do not commit credentials or signing keys to the repository.

The API project is:

`src/Byway.API/Byway.API.csproj`

Run it with:

```powershell
dotnet run --project src/Byway.API/Byway.API.csproj
```

When running with the local configuration, Swagger is available at:

https://localhost:61550/

Your local launch settings may use a different URL or port.

### 2. Run the Frontend

Open a second terminal from the repository root:

```powershell
cd frontend/byway-react
npm install
npm run dev
```

Vite prints the local frontend URL in the terminal, typically:

http://localhost:5173/

Make sure the API is running and the frontend API configuration points to the correct backend URL.

## Notes

- Database credentials and JWT signing keys must be configured locally.
- `node_modules`, build outputs, and IDE-specific files should not be committed.
- This project is intended for learning and portfolio demonstration.

## License

No license has been specified yet.
