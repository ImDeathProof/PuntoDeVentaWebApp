using PuntoDeVentaWeb.Models;

public interface IProductRepository
{
    Task AddProductAsync(Product product);
    Task<Product> GetProductByIdAsync(int productId);
    Task<IEnumerable<Product>> GetProductsAsync();
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(Product product);
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    Task removeStockAsync(int productId, int quantity);
    Task addStockAsync(int productId, int quantity);
    bool checkIfProductExistsInPurchaseAsync(int productId);
    bool checkIfProductExistsInSaleAsync(int productId);
    Task SaveAsync();
}