namespace CoffeeShop.Models;

public class Payment
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }

    public Order Order { get; set; } = null!;
}
