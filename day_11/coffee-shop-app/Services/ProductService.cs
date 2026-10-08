using CoffeeShop.Data;
using CoffeeShop.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Services;

public class ProductService
{
    public async Task<List<Category>> GetCategoriesAsync()
    {
        await using var context = new CoffeeShopContext();
        return await context.Categories
            .OrderBy(category => category.CategoryId)
            .ToListAsync();
    }

    public async Task<Product> AddProductAsync(string name, string description, decimal price, int categoryId, bool isAvailable)
    {
        await using var context = new CoffeeShopContext();

        var category = await context.Categories.FindAsync(categoryId);
        if (category is null)
        {
            throw new InvalidOperationException($"Category {categoryId} was not found.");
        }

        var product = new Product
        {
            Name = name.Trim(),
            Description = description.Trim(),
            Price = price,
            CategoryId = categoryId,
            IsAvailable = isAvailable
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();
        return product;
    }

    public async Task<List<Product>> GetProductsAsync(bool availableOnly = false)
    {
        await using var context = new CoffeeShopContext();

        var query = context.Products
            .Include(product => product.Category)
            .AsQueryable();

        if (availableOnly)
        {
            query = query.Where(product => product.IsAvailable);
        }

        return await query
            .OrderBy(product => product.ProductId)
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int productId)
    {
        await using var context = new CoffeeShopContext();
        return await context.Products
            .Include(product => product.Category)
            .FirstOrDefaultAsync(product => product.ProductId == productId);
    }

    public async Task<Product?> UpdateProductAsync(int productId, string name, string description, decimal price, bool isAvailable)
    {
        await using var context = new CoffeeShopContext();

        var product = await context.Products.FindAsync(productId);
        if (product is null)
        {
            return null;
        }

        product.Name = name.Trim();
        product.Description = description.Trim();
        product.Price = price;
        product.IsAvailable = isAvailable;

        await context.SaveChangesAsync();
        return product;
    }

    public async Task<string?> DeleteProductAsync(int productId)
    {
        await using var context = new CoffeeShopContext();

        var product = await context.Products.FindAsync(productId);
        if (product is null)
        {
            return $"Product {productId} was not found.";
        }

        var isUsedInOrders = await context.OrderItems.AnyAsync(orderItem => orderItem.ProductId == productId);
        if (isUsedInOrders)
        {
            return "This product cannot be deleted because it is part of an existing order.";
        }

        try
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync();
            return null;
        }
        catch (DbUpdateException)
        {
            return "The product could not be deleted because it is still referenced by other records.";
        }
    }
}
