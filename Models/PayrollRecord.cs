
namespace PayrollApp.Models;

public class PayrollRecord
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string? Period { get; set; }
    public decimal GrossPay { get; set; }
    public decimal Taxes { get; set; }
    public decimal NetPay { get; set; }
    public DateTime CreatedAt { get; set; }
}
