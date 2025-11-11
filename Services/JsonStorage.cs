using System.Text.Json;
using System.Collections.Generic;

public class JsonStorage
{
    private readonly string employeeFile = "data/employees.json";
    private readonly string payrollFile = "data/payrolls.json";

    public JsonStorage()
    {
        Directory.CreateDirectory("data");

        if (!File.Exists(employeeFile))
            File.WriteAllText(employeeFile, "[]");

        if (!File.Exists(payrollFile))
            File.WriteAllText(payrollFile, "[]");
    }

    // ==================== EMPLOYEES ====================

    public List<Employee> GetEmployees()
    {
        var json = File.ReadAllText(employeeFile);
        return JsonSerializer.Deserialize<List<Employee>>(json) ?? new List<Employee>();
    }

    public void AddEmployee(Employee emp)
    {
        var list = GetEmployees();
        emp.Id = list.Count > 0 ? list.Max(e => e.Id) + 1 : 1;
        list.Add(emp);
        File.WriteAllText(employeeFile, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));
    }

    // ==================== PAYROLLS ====================

    public List<Payroll> GetPayrolls()
    {
        var json = File.ReadAllText(payrollFile);
        return JsonSerializer.Deserialize<List<Payroll>>(json) ?? new List<Payroll>();
    }

    public void AddPayroll(Payroll pay)
    {
        var list = GetPayrolls();
        pay.Id = list.Count > 0 ? list.Max(p => p.Id) + 1 : 1;
        list.Add(pay);
        File.WriteAllText(payrollFile, JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true }));
    }
}
