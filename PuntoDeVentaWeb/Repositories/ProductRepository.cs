
using Microsoft.EntityFrameworkCore;
using PuntoDeVentaWeb.Data;
using PuntoDeVentaWeb.Models;

public class ProductRepository : IProductRepository
{
    private readonly DataContext _context;
    public ProductRepository(DataContext context)
    {
        _context = context;
    }

    public async Task AddProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task addStockAsync(int productId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) throw new Exception("Product not found");
        product.Stock += quantity;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public bool checkIfProductExistsInPurchaseAsync(int productId)
    {
        bool exists = _context.PurchaseDetails.Any(p => p.ProductId == productId);
        return exists;
    }

    public bool checkIfProductExistsInSaleAsync(int productId)
    {
        bool exists = _context.SaleDetails.Any(s => s.ProductId == productId);
        return exists;
    }

    public async Task DeleteProductAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task<Product> GetProductByIdAsync(int productId)
    {
        if (productId <= 0) throw new ArgumentException("Invalid product ID");
        return await _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == productId)??
            throw new KeyNotFoundException("Product not found");
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId)
    {
        if (brandId <= 0) throw new ArgumentException("Invalid brand ID");
        return await _context.Products
            .Where(p => p.BrandId == brandId)
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        if (categoryId <= 0) throw new ArgumentException("Invalid category ID");
        return await _context.Products
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task removeStockAsync(int productId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) throw new Exception("Product not found");
        product.Stock -= quantity;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetProductsAsync();
        return await _context.Products
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .ToListAsync();
    }

    public async Task UpdateProductAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
}