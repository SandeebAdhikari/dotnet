using CoffeeShop.Data;
using CoffeeShop.Models;
using CoffeeShop.Services;
using Microsoft.EntityFrameworkCore;

var application = new CoffeeShopApplication();
await application.RunAsync();

public class CoffeeShopApplication
{
    private readonly CustomerService _customerService = new();
    private readonly ProductService _productService = new();
    private readonly OrderService _orderService = new();

    public async Task RunAsync()
    {
        if (!await TryInitializeDatabaseAsync())
        {
            return;
        }

        var isRunning = true;
        while (isRunning)
        {
            DisplayMainMenu();
            var choice = ReadRequiredText("Select an option: ");

            try
            {
                isRunning = await HandleMenuChoiceAsync(choice);
            }
            catch (Exception exception)
            {
                Console.WriteLine();
                Console.WriteLine($"Error: {exception.Message}");
                Pause();
            }
        }
    }

    private static async Task<bool> TryInitializeDatabaseAsync()
    {
        try
        {
            await using var context = new CoffeeShopContext();
            await context.Database.MigrateAsync();
            await context.EnsureSeedDataAsync();
            return true;
        }
        catch (Exception exception)
        {
            Console.WriteLine("Could not connect to SQL Server or apply migrations.");
            Console.WriteLine("Update the CoffeeShop connection string in appsettings.json, then run the application again.");
            Console.WriteLine();
            Console.WriteLine(exception.Message);
            return false;
        }
    }

    private static void DisplayMainMenu()
    {
        Console.Clear();
        Console.WriteLine("================================");
        Console.WriteLine("       COFFEE SHOP SYSTEM");
        Console.WriteLine("================================");
        Console.WriteLine();
        Console.WriteLine("1. Add Customer");
        Console.WriteLine("2. View Customers");
        Console.WriteLine("3. Add Product");
        Console.WriteLine("4. View Products");
        Console.WriteLine("5. Create Order");
        Console.WriteLine("6. View Order Details");
        Console.WriteLine("7. View Customer Order History");
        Console.WriteLine("8. Record Payment");
        Console.WriteLine("9. Update Product");
        Console.WriteLine("10. Delete Product");
        Console.WriteLine("0. Exit");
        Console.WriteLine();
    }

    private async Task<bool> HandleMenuChoiceAsync(string choice)
    {
        switch (choice)
        {
            case "1":
                await AddCustomerAsync();
                return true;
            case "2":
                await ViewCustomersAsync();
                return true;
            case "3":
                await AddProductAsync();
                return true;
            case "4":
                await ViewProductsAsync();
                return true;
            case "5":
                await CreateOrderAsync();
                return true;
            case "6":
                await ViewOrderDetailsAsync();
                return true;
            case "7":
                await ViewCustomerOrderHistoryAsync();
                return true;
            case "8":
                await RecordPaymentAsync();
                return true;
            case "9":
                await UpdateProductAsync();
                return true;
            case "10":
                await DeleteProductAsync();
                return true;
            case "0":
                Console.WriteLine("Goodbye.");
                return false;
            default:
                Console.WriteLine("Please choose a valid menu option.");
                Pause();
                return true;
        }
    }

    private async Task AddCustomerAsync()
    {
        Console.WriteLine();
        Console.WriteLine("Add Customer");
        Console.WriteLine("------------");

        var firstName = ReadRequiredText("First Name: ");
        var lastName = ReadRequiredText("Last Name: ");
        var email = ReadEmail("Email: ");
        var phone = ReadRequiredText("Phone: ");

        var customer = await _customerService.AddCustomerAsync(firstName, lastName, email, phone);
        Console.WriteLine($"Customer #{customer.CustomerId} was saved.");
        Pause();
    }

    private async Task ViewCustomersAsync()
    {
        Console.WriteLine();
        Console.WriteLine("Customers");
        Console.WriteLine("---------");

        var customers = await _customerService.GetCustomersAsync();
        if (customers.Count == 0)
        {
            Console.WriteLine("No customers found.");
            Pause();
            return;
        }

        Console.WriteLine($"{"ID",-6}{"Name",-24}{"Email",-28}{"Phone",-16}");
        foreach (var customer in customers)
        {
            var fullName = $"{customer.FirstName} {customer.LastName}";
            Console.WriteLine($"{customer.CustomerId,-6}{fullName,-24}{customer.Email,-28}{customer.Phone,-16}");
        }

        Pause();
    }

    private async Task AddProductAsync()
    {
        Console.WriteLine();
        Console.WriteLine("Add Product");
        Console.WriteLine("-----------");

        var categories = await DisplayCategoriesAsync();
        if (categories.Count == 0)
        {
            return;
        }

        var name = ReadRequiredText("Name: ");
        var description = ReadRequiredText("Description: ");
        var price = ReadPositiveDecimal("Price: ");
        var categoryId = ReadExistingId("Category ID: ", categories.Select(category => category.CategoryId).ToHashSet());
        var isAvailable = ReadYesNo("Available (y/n): ");

        var product = await _productService.AddProductAsync(name, description, price, categoryId, isAvailable);
        Console.WriteLine($"Product #{product.ProductId} was saved.");
        Pause();
    }

    private async Task ViewProductsAsync()
    {
        Console.WriteLine();
        Console.WriteLine("Products");
        Console.WriteLine("--------");

        var products = await _productService.GetProductsAsync();
        DisplayProducts(products);
        Pause();
    }

    private async Task CreateOrderAsync()
    {
        Console.WriteLine();
        Console.WriteLine("Create Order");
        Console.WriteLine("------------");

        var customers = await _customerService.GetCustomersAsync();
        if (customers.Count == 0)
        {
            Console.WriteLine("Add a customer before creating an order.");
            Pause();
            return;
        }

        Console.WriteLine($"{"ID",-6}{"Name",-24}{"Email",-28}");
        foreach (var customer in customers)
        {
            var fullName = $"{customer.FirstName} {customer.LastName}";
            Console.WriteLine($"{customer.CustomerId,-6}{fullName,-24}{customer.Email,-28}");
        }

        var customerId = ReadExistingId("Customer ID: ", customers.Select(customer => customer.CustomerId).ToHashSet());

        var availableProducts = await _productService.GetProductsAsync(availableOnly: true);
        if (availableProducts.Count == 0)
        {
            Console.WriteLine("There are no available products to order.");
            Pause();
            return;
        }

        var items = new List<(int ProductId, int Quantity)>();
        var addMore = true;
        while (addMore)
        {
            Console.WriteLine();
            DisplayProducts(availableProducts);

            var productId = ReadExistingId("Product ID: ", availableProducts.Select(product => product.ProductId).ToHashSet());
            var quantity = ReadPositiveInt("Quantity: ");
            items.Add((productId, quantity));

            addMore = ReadYesNo("Add another product? (y/n): ");
        }

        var order = await _orderService.CreateOrderAsync(customerId, items);
        Console.WriteLine($"Order #{order.OrderId} was saved. Total: {order.TotalAmount:C}");
        Pause();
    }

    private async Task ViewOrderDetailsAsync()
    {
        Console.WriteLine();
        Console.WriteLine("View Order Details");
        Console.WriteLine("------------------");

        var orderId = ReadPositiveInt("Order ID: ");
        var order = await _orderService.GetOrderDetailsAsync(orderId);
        if (order is null)
        {
            Console.WriteLine($"Order {orderId} was not found.");
            Pause();
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"Order ID: {order.OrderId}");
        Console.WriteLine($"Customer: {order.Customer.FirstName} {order.Customer.LastName}");
        Console.WriteLine($"Order Date: {order.OrderDate:g}");
        Console.WriteLine($"Order Status: {order.Status}");
        Console.WriteLine();
        Console.WriteLine("Items:");
        Console.WriteLine($"{"Product",-20}{"Quantity",-12}{"Unit Price",-14}{"Line Total",-12}");

        foreach (var item in order.OrderItems)
        {
            var lineTotal = item.UnitPrice * item.Quantity;
            Console.WriteLine($"{item.Product.Name,-20}{item.Quantity,-12}{item.UnitPrice,-14:C}{lineTotal,-12:C}");
        }

        Console.WriteLine();
        Console.WriteLine($"Total Amount: {order.TotalAmount:C}");
        Console.WriteLine(order.Payment is null
            ? "Payment Status: Unpaid"
            : $"Payment Status: Paid ({order.Payment.PaymentMethod} on {order.Payment.PaymentDate:g})");
        Pause();
    }

    private async Task ViewCustomerOrderHistoryAsync()
    {
        Console.WriteLine();
        Console.WriteLine("Customer Order History");
        Console.WriteLine("----------------------");

        var customerId = ReadPositiveInt("Customer ID: ");
        var customer = await _customerService.GetCustomerByIdAsync(customerId);
        if (customer is null)
        {
            Console.WriteLine($"Customer {customerId} was not found.");
            Pause();
            return;
        }

        var orders = await _orderService.GetCustomerOrderHistoryAsync(customerId);
        Console.WriteLine();
        Console.WriteLine($"Orders for {customer.FirstName} {customer.LastName}");

        if (orders.Count == 0)
        {
            Console.WriteLine("This customer has no orders yet.");
            Pause();
            return;
        }

        Console.WriteLine($"{"Order ID",-12}{"Date",-22}{"Status",-12}{"Total",-12}{"Payment",-24}");
        foreach (var order in orders)
        {
            var paymentInfo = order.Payment is null
                ? "Unpaid"
                : $"{order.Payment.PaymentMethod} ({order.Payment.Amount:C})";

            Console.WriteLine($"{order.OrderId,-12}{order.OrderDate,-22:g}{order.Status,-12}{order.TotalAmount,-12:C}{paymentInfo,-24}");
        }

        Pause();
    }

    private async Task RecordPaymentAsync()
    {
        Console.WriteLine();
        Console.WriteLine("Record Payment");
        Console.WriteLine("--------------");

        var orderId = ReadPositiveInt("Order ID: ");
        var paymentMethod = ReadPaymentMethod();

        var payment = await _orderService.RecordPaymentAsync(orderId, paymentMethod);
        Console.WriteLine($"Payment #{payment.PaymentId} recorded for {payment.Amount:C} using {payment.PaymentMethod}.");
        Pause();
    }

    private async Task UpdateProductAsync()
    {
        Console.WriteLine();
        Console.WriteLine("Update Product");
        Console.WriteLine("--------------");

        var products = await _productService.GetProductsAsync();
        DisplayProducts(products);
        if (products.Count == 0)
        {
            Pause();
            return;
        }

        var productId = ReadPositiveInt("Product ID to update: ");
        var product = await _productService.GetProductByIdAsync(productId);
        if (product is null)
        {
            Console.WriteLine($"Product {productId} was not found.");
            Pause();
            return;
        }

        Console.WriteLine($"Current Name: {product.Name}");
        var name = ReadRequiredText("New Name: ");
        Console.WriteLine($"Current Description: {product.Description}");
        var description = ReadRequiredText("New Description: ");
        Console.WriteLine($"Current Price: {product.Price:C}");
        var price = ReadPositiveDecimal("New Price: ");
        Console.WriteLine($"Current Availability: {(product.IsAvailable ? "Yes" : "No")}");
        var isAvailable = ReadYesNo("Available (y/n): ");

        var updatedProduct = await _productService.UpdateProductAsync(productId, name, description, price, isAvailable);
        if (updatedProduct is null)
        {
            Console.WriteLine($"Product {productId} was not found.");
        }
        else
        {
            Console.WriteLine($"Product #{updatedProduct.ProductId} was updated.");
        }

        Pause();
    }

    private async Task DeleteProductAsync()
    {
        Console.WriteLine();
        Console.WriteLine("Delete Product");
        Console.WriteLine("--------------");

        var products = await _productService.GetProductsAsync();
        DisplayProducts(products);
        if (products.Count == 0)
        {
            Pause();
            return;
        }

        var productId = ReadPositiveInt("Product ID to delete: ");
        if (!ReadYesNo($"Delete product {productId}? (y/n): "))
        {
            Console.WriteLine("Delete cancelled.");
            Pause();
            return;
        }

        var errorMessage = await _productService.DeleteProductAsync(productId);
        Console.WriteLine(errorMessage ?? $"Product #{productId} was deleted.");
        Pause();
    }

    private async Task<List<Category>> DisplayCategoriesAsync()
    {
        var categories = await _productService.GetCategoriesAsync();
        if (categories.Count == 0)
        {
            Console.WriteLine("No categories found. Apply the database migration and restart.");
            Pause();
            return categories;
        }

        Console.WriteLine($"{"ID",-6}{"Name",-16}{"Description"}");
        foreach (var category in categories)
        {
            Console.WriteLine($"{category.CategoryId,-6}{category.Name,-16}{category.Description}");
        }

        return categories;
    }

    private static void DisplayProducts(List<Product> products)
    {
        if (products.Count == 0)
        {
            Console.WriteLine("No products found.");
            return;
        }

        Console.WriteLine($"{"ID",-6}{"Product Name",-20}{"Category",-14}{"Price",-10}{"Availability"}");
        foreach (var product in products)
        {
            var availability = product.IsAvailable ? "Available" : "Unavailable";
            Console.WriteLine($"{product.ProductId,-6}{product.Name,-20}{product.Category.Name,-14}{product.Price,-10:C}{availability}");
        }
    }

    private static string ReadRequiredText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var value = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Trim();
            }

            Console.WriteLine("This value is required.");
        }
    }

    private static string ReadEmail(string prompt)
    {
        while (true)
        {
            var email = ReadRequiredText(prompt);
            if (email.Contains('@') && email.Contains('.'))
            {
                return email;
            }

            Console.WriteLine("Enter a valid email address.");
        }
    }

    private static int ReadPositiveInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out var value) && value > 0)
            {
                return value;
            }

            Console.WriteLine("Enter a whole number greater than zero.");
        }
    }

    private static decimal ReadPositiveDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (decimal.TryParse(Console.ReadLine(), out var value) && value >= 0)
            {
                return Math.Round(value, 2);
            }

            Console.WriteLine("Enter a valid amount of 0 or greater.");
        }
    }

    private static int ReadExistingId(string prompt, HashSet<int> validIds)
    {
        while (true)
        {
            var id = ReadPositiveInt(prompt);
            if (validIds.Contains(id))
            {
                return id;
            }

            Console.WriteLine("That ID is not in the list.");
        }
    }

    private static bool ReadYesNo(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var value = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (value is "y" or "yes")
            {
                return true;
            }

            if (value is "n" or "no")
            {
                return false;
            }

            Console.WriteLine("Enter y or n.");
        }
    }

    private static string ReadPaymentMethod()
    {
        while (true)
        {
            Console.Write("Payment Method (Cash/Card): ");
            var value = Console.ReadLine()?.Trim();
            if (string.Equals(value, "Cash", StringComparison.OrdinalIgnoreCase))
            {
                return "Cash";
            }

            if (string.Equals(value, "Card", StringComparison.OrdinalIgnoreCase))
            {
                return "Card";
            }

            Console.WriteLine("Enter Cash or Card.");
        }
    }

    private static void Pause()
    {
        Console.WriteLine();
        Console.Write("Press Enter to continue...");
        Console.ReadLine();
    }
}
