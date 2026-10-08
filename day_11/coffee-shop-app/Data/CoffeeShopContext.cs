using CoffeeShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CoffeeShop.Data;

public class CoffeeShopContext : DbContext
{
    public CoffeeShopContext()
    {
    }

    public CoffeeShopContext(DbContextOptions<CoffeeShopContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        var basePath = File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"))
            ? Directory.GetCurrentDirectory()
            : AppContext.BaseDirectory;

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var connectionString = configuration.GetConnectionString("CoffeeShop");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'CoffeeShop' was not found. Update appsettings.json with your SQL Server connection information.");
        }

        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureCustomer(modelBuilder);
        ConfigureCategory(modelBuilder);
        ConfigureProduct(modelBuilder);
        ConfigureOrder(modelBuilder);
        ConfigureOrderItem(modelBuilder);
        ConfigurePayment(modelBuilder);
        SeedCatalog(modelBuilder);
    }

    public async Task EnsureSeedDataAsync()
    {
        if (await Categories.AnyAsync())
        {
            return;
        }

        var coffee = new Category
        {
            Name = "Coffee",
            Description = "Espresso-based and brewed coffee drinks"
        };
        var tea = new Category
        {
            Name = "Tea",
            Description = "Hot and iced teas"
        };
        var bakery = new Category
        {
            Name = "Bakery",
            Description = "Freshly baked pastries and snacks"
        };

        Categories.AddRange(coffee, tea, bakery);
        await SaveChangesAsync();

        Products.AddRange(
            new Product { Name = "Cappuccino", Description = "Espresso with steamed milk and foam", Price = 4.50m, IsAvailable = true, CategoryId = coffee.CategoryId },
            new Product { Name = "Latte", Description = "Espresso with steamed milk", Price = 4.75m, IsAvailable = true, CategoryId = coffee.CategoryId },
            new Product { Name = "Americano", Description = "Espresso diluted with hot water", Price = 3.50m, IsAvailable = true, CategoryId = coffee.CategoryId },
            new Product { Name = "Espresso", Description = "Rich concentrated coffee shot", Price = 2.75m, IsAvailable = true, CategoryId = coffee.CategoryId },
            new Product { Name = "Green Tea", Description = "Light and refreshing green tea", Price = 3.25m, IsAvailable = true, CategoryId = tea.CategoryId },
            new Product { Name = "Croissant", Description = "Buttery flaky pastry", Price = 3.00m, IsAvailable = true, CategoryId = bakery.CategoryId },
            new Product { Name = "Muffin", Description = "Fresh baked blueberry muffin", Price = 2.50m, IsAvailable = true, CategoryId = bakery.CategoryId });

        await SaveChangesAsync();
    }

    private static void ConfigureCustomer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(customer => customer.CustomerId);
            entity.Property(customer => customer.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(customer => customer.LastName).IsRequired().HasMaxLength(50);
            entity.Property(customer => customer.Email).IsRequired().HasMaxLength(100);
            entity.Property(customer => customer.Phone).IsRequired().HasMaxLength(20);
            entity.Property(customer => customer.CreatedAt).IsRequired();

            entity.HasMany(customer => customer.Orders)
                .WithOne(order => order.Customer)
                .HasForeignKey(order => order.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(category => category.CategoryId);
            entity.Property(category => category.Name).IsRequired().HasMaxLength(50);
            entity.Property(category => category.Description).HasMaxLength(250);

            entity.HasMany(category => category.Products)
                .WithOne(product => product.Category)
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigureProduct(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(product => product.ProductId);
            entity.Property(product => product.Name).IsRequired().HasMaxLength(100);
            entity.Property(product => product.Description).HasMaxLength(250);
            entity.Property(product => product.Price).HasColumnType("decimal(10,2)");
            entity.Property(product => product.IsAvailable).IsRequired();
        });
    }

    private static void ConfigureOrder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(order => order.OrderId);
            entity.Property(order => order.OrderDate).IsRequired();
            entity.Property(order => order.TotalAmount).HasColumnType("decimal(10,2)");
            entity.Property(order => order.Status).IsRequired().HasMaxLength(20);

            entity.HasMany(order => order.OrderItems)
                .WithOne(orderItem => orderItem.Order)
                .HasForeignKey(orderItem => orderItem.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(order => order.Payment)
                .WithOne(payment => payment.Order)
                .HasForeignKey<Payment>(payment => payment.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureOrderItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(orderItem => orderItem.OrderItemId);
            entity.Property(orderItem => orderItem.Quantity).IsRequired();
            entity.Property(orderItem => orderItem.UnitPrice).HasColumnType("decimal(10,2)");

            entity.HasOne(orderItem => orderItem.Product)
                .WithMany(product => product.OrderItems)
                .HasForeignKey(orderItem => orderItem.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigurePayment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(payment => payment.PaymentId);
            entity.Property(payment => payment.Amount).HasColumnType("decimal(10,2)");
            entity.Property(payment => payment.PaymentMethod).IsRequired().HasMaxLength(20);
            entity.Property(payment => payment.PaymentDate).IsRequired();
            entity.HasIndex(payment => payment.OrderId).IsUnique();
        });
    }

    private static void SeedCatalog(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, Name = "Coffee", Description = "Espresso-based and brewed coffee drinks" },
            new Category { CategoryId = 2, Name = "Tea", Description = "Hot and iced teas" },
            new Category { CategoryId = 3, Name = "Bakery", Description = "Freshly baked pastries and snacks" });

        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, Name = "Cappuccino", Description = "Espresso with steamed milk and foam", Price = 4.50m, IsAvailable = true, CategoryId = 1 },
            new Product { ProductId = 2, Name = "Latte", Description = "Espresso with steamed milk", Price = 4.75m, IsAvailable = true, CategoryId = 1 },
            new Product { ProductId = 3, Name = "Americano", Description = "Espresso diluted with hot water", Price = 3.50m, IsAvailable = true, CategoryId = 1 },
            new Product { ProductId = 4, Name = "Espresso", Description = "Rich concentrated coffee shot", Price = 2.75m, IsAvailable = true, CategoryId = 1 },
            new Product { ProductId = 5, Name = "Green Tea", Description = "Light and refreshing green tea", Price = 3.25m, IsAvailable = true, CategoryId = 2 },
            new Product { ProductId = 6, Name = "Croissant", Description = "Buttery flaky pastry", Price = 3.00m, IsAvailable = true, CategoryId = 3 },
            new Product { ProductId = 7, Name = "Muffin", Description = "Fresh baked blueberry muffin", Price = 2.50m, IsAvailable = true, CategoryId = 3 });
    }
}
