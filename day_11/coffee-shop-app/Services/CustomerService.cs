using CoffeeShop.Data;
using CoffeeShop.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Services;

public class CustomerService
{
    public async Task<Customer> AddCustomerAsync(string firstName, string lastName, string email, string phone)
    {
        await using var context = new CoffeeShopContext();

        var customer = new Customer
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.Trim(),
            Phone = phone.Trim(),
            CreatedAt = DateTime.Now
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();
        return customer;
    }

    public async Task<List<Customer>> GetCustomersAsync()
    {
        await using var context = new CoffeeShopContext();
        return await context.Customers
            .OrderBy(customer => customer.CustomerId)
            .ToListAsync();
    }

    public async Task<Customer?> GetCustomerByIdAsync(int customerId)
    {
        await using var context = new CoffeeShopContext();
        return await context.Customers.FindAsync(customerId);
    }
}
