# DevOps Task Manager - Stage 1 (Local Setup)

This repository contains the first stage of the DevOps hands-on roadmap: a fully functional local full-stack web application with complete architectural separation.

## Architecture

Client Browser -> React (Vite) -> ASP.NET Core Web API (.NET 8) -> EF Core 8 -> PostgreSQL

## Prerequisites

Ensure the following tools are installed on your machine:
* **Node.js**: v18+ and `npm`
* **.NET 8 SDK**: `dotnet --version` outputs `8.0.x`
* **PostgreSQL**: 14+ running locally on port `5432`
* **EF Core CLI**: Install globally via:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

---

## 1. Database Setup

1. Start your local PostgreSQL server on port `5432`.
2. Connect using `psql` or pgAdmin and run:
   ```sql
   CREATE USER taskuser WITH PASSWORD 'taskpassword';
   CREATE DATABASE taskmanagerdb OWNER taskuser;
   GRANT ALL PRIVILEGES ON DATABASE taskmanagerdb TO taskuser;
   ```
3. Check `backend/appsettings.Development.json` to verify the connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=taskmanagerdb;Username=taskuser;Password=taskpassword"
   }
   ```

---

## 2. Backend Setup & Migrations

1. Open a terminal and navigate to the backend directory:
   ```bash
   cd backend
   ```
2. Restore package dependencies:
   ```bash
   dotnet restore
   ```
3. Create the initial EF Core migration:
   ```bash
   dotnet ef migrations add InitialCreate
   ```
4. Apply the migration to the PostgreSQL database:
   ```bash
   dotnet ef database update
   ```
5. Run the backend Web API:
   ```bash
   dotnet run --urls=http://localhost:5000
   ```
   * The API runs at `http://localhost:5000`
   * Swagger documentation: `http://localhost:5000/swagger`

---

## 3. Frontend Setup

1. Open a separate terminal and navigate to the frontend directory:
   ```bash
   cd frontend
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Verify `frontend/.env.development` points to your backend:
   ```ini
   VITE_API_BASE_URL=http://localhost:5000
   ```
4. Start the Vite development server:
   ```bash
   npm run dev
   ```
   * The React UI is accessible at `http://localhost:5173`

---

## 4. API Endpoints

| Verb | Path | Description |
|---|---|---|
| `GET` | `/api/tasks` | Returns all tasks |
| `GET` | `/api/tasks/{id}` | Returns a single task |
| `POST` | `/api/tasks` | Creates a new task |
| `PUT` | `/api/tasks/{id}` | Updates title, description, or completion state |
| `DELETE` | `/api/tasks/{id}` | Deletes a task |

---

## 5. Running the Complete System Locally

Start the processes in this order:
1. **PostgreSQL**: Serving on `localhost:5432`
2. **Backend**: `cd backend && dotnet run --urls=http://localhost:5000`
3. **Frontend**: `cd frontend && npm run dev`
4. **Browser**: Open `http://localhost:5173` to test adding, toggling, editing, and deleting tasks.
