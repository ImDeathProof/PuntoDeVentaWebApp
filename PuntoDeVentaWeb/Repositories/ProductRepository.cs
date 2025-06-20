
using Microsoft.EntityFrameworkCore;
using PuntoDeVentaWeb.Data;

public class ProductRepository : IProductRepository
{
    private readonly DataContext _context;
    public ProductRepository(DataContext context)
    {
        _context = context;
    }

    public async Task addStockAsync(int productId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) throw new Exception("Product not found");
        product.Stock += quantity;
        _context.Products.Update(product);
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

    public async Task removeStockAsync(int productId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) throw new Exception("Product not found");
        product.Stock -= quantity;
        _context.Products.Update(product);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}