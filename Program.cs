
using System.Text.Json;
using PayrollApp.Models;
using PayrollApp.Services;

var builder = WebApplication.CreateBuilder(args);

var dataFolder = builder.Configuration[""DATA_FOLDER""] ?? ""data"";
Directory.CreateDirectory(dataFolder);

builder.Services.AddSingleton(new JsonStorage(dataFolder));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => 
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Serve static files (frontend)
app.UseDefaultFiles();
app.UseStaticFiles();

var storage = app.Services.GetRequiredService<JsonStorage>();

app.MapGet("/employees", () => Results.Ok(storage.GetAll<Employee>(""employees.json"")));
app.MapGet("/employees/{id}", (Guid id) =>
{
    var e = storage.Find<Employee>(""employees.json"", id);
    return e is not null ? Results.Ok(e) : Results.NotFound();
});
app.MapPost("/employees", (Employee emp) =>
{
    emp.Id = Guid.NewGuid();
    storage.Add("employees.json", emp);
    return Results.Created($"/employees/{emp.Id}", emp);
});
app.MapPut("/employees/{id}", (Guid id, Employee update) =>
{
    var ok = storage.Update("employees.json", id, update);
    return ok ? Results.NoContent() : Results.NotFound();
});
app.MapDelete("/employees/{id}", (Guid id) =>
{
    var ok = storage.Delete("employees.json", id);
    return ok ? Results.NoContent() : Results.NotFound();
});

app.MapGet("/payrolls", () => Results.Ok(storage.GetAll<PayrollRecord>(""payrolls.json"")));
app.MapPost("/payrolls/calculate", (PayrollRequest req) =>
{
    var emp = storage.Find<Employee>(""employees.json"", req.EmployeeId);
    if (emp is null) return Results.NotFound($"Employee {req.EmployeeId} not found");

    decimal gross = emp.BaseSalary + (emp.Allowances ?? 0) - (emp.Deductions ?? 0);
    decimal taxRate = req.TaxRate ?? emp.TaxRate ?? 0.10m;
    decimal taxes = Math.Round(gross * taxRate, 2);
    decimal net = gross - taxes;

    var record = new PayrollRecord
    {
        Id = Guid.NewGuid(),
        EmployeeId = emp.Id,
        Period = req.Period ?? DateTime.UtcNow.ToString("yyyy-MM"),
        GrossPay = gross,
        Taxes = taxes,
        NetPay = net,
        CreatedAt = DateTime.UtcNow
    };

    storage.Add("payrolls.json", record);
    return Results.Ok(record);
});

app.Run();

public record PayrollRequest(Guid EmployeeId, string? Period, decimal? TaxRate);
