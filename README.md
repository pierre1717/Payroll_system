
# PayrollApp (C# minimal API + SPA)

Minimal payroll management API with a static single-page frontend (green & white). JSON files are used as storage so it's easy to deploy to Render with Docker.

Run locally:
- Install .NET 8 SDK
- dotnet run
- Visit http://localhost:5000 (or port configured)

Docker:
- docker build -t payrollapp:latest .
- docker run -p 8080:8080 -e DATA_FOLDER=/app/data payrollapp:latest

Notes: JSON storage is NOT suitable for heavy production use. Use a real DB for concurrency and scaling.
