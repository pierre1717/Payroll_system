using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PayrollApp.Models;
using PayrollApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Ajouter le service JSON
builder.Services.AddSingleton<JsonStorage>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

var json = app.Services.GetRequiredService<JsonStorage>();

// --- API employés ---
app.MapGet("/api/employees", () => json.GetEmployees());
app.MapPost("/api/employees", (Employee emp) => json.AddEmployee(emp));

// --- API paies ---
app.MapGet("/api/payrolls", () => json.GetPayrolls());
app.MapPost("/api/payrolls", (PayrollRecord rec) => json.AddPayroll(rec));

// --- Page d'accueil ---
app.MapGet("/", async context =>
{
    context.Response.Redirect("/index.html");
});

app.Run();
