using CoffeeShop.Data;
using CoffeeShop.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Services;

public class OrderService
{
    public async Task<Order> CreateOrderAsync(int customerId, List<(int ProductId, int Quantity)> items)
    {
        if (items.Count == 0)
        {
            throw new InvalidOperationException("An order must contain at least one product.");
        }

        await using var context = new CoffeeShopContext();

        var customer = await context.Customers.FindAsync(customerId);
        if (customer is null)
        {
            throw new InvalidOperationException($"Customer {customerId} was not found.");
        }

        var order = new Order
        {
            CustomerId = customerId,
            OrderDate = DateTime.Now,
            Status = "Pending",
            TotalAmount = 0m
        };

        context.Orders.Add(order);

        decimal totalAmount = 0m;
        var groupedItems = items
            .GroupBy(item => item.ProductId)
            .Select(group => (ProductId: group.Key, Quantity: group.Sum(item => item.Quantity)));

        foreach (var (productId, quantity) in groupedItems)
        {
            var product = await context.Products.FindAsync(productId);
            if (product is null)
            {
                throw new InvalidOperationException($"Product {productId} was not found.");
            }

            if (!product.IsAvailable)
            {
                throw new InvalidOperationException($"{product.Name} is not available.");
            }

            if (quantity <= 0)
            {
                throw new InvalidOperationException("Quantity must be greater than zero.");
            }

            var orderItem = new OrderItem
            {
                ProductId = product.ProductId,
                Quantity = quantity,
                UnitPrice = product.Price
            };

            order.OrderItems.Add(orderItem);
            totalAmount += orderItem.UnitPrice * orderItem.Quantity;
        }

        order.TotalAmount = totalAmount;
        await context.SaveChangesAsync();
        return order;
    }

    public async Task<Order?> GetOrderDetailsAsync(int orderId)
    {
        await using var context = new CoffeeShopContext();
        return await context.Orders
            .Include(order => order.Customer)
            .Include(order => order.Payment)
            .Include(order => order.OrderItems)
                .ThenInclude(orderItem => orderItem.Product)
            .FirstOrDefaultAsync(order => order.OrderId == orderId);
    }

    public async Task<List<Order>> GetCustomerOrderHistoryAsync(int customerId)
    {
        await using var context = new CoffeeShopContext();
        return await context.Orders
            .Include(order => order.Payment)
            .Where(order => order.CustomerId == customerId)
            .OrderByDescending(order => order.OrderDate)
            .ToListAsync();
    }

    public async Task<Payment> RecordPaymentAsync(int orderId, string paymentMethod)
    {
        await using var context = new CoffeeShopContext();

        var order = await context.Orders
            .Include(existingOrder => existingOrder.Payment)
            .FirstOrDefaultAsync(existingOrder => existingOrder.OrderId == orderId);

        if (order is null)
        {
            throw new InvalidOperationException($"Order {orderId} was not found.");
        }

        if (order.Payment is not null || string.Equals(order.Status, "Paid", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("This order already has a payment.");
        }

        var payment = new Payment
        {
            OrderId = order.OrderId,
            Amount = order.TotalAmount,
            PaymentMethod = paymentMethod,
            PaymentDate = DateTime.Now
        };

        context.Payments.Add(payment);
        order.Status = "Paid";
        await context.SaveChangesAsync();
        return payment;
    }
}
