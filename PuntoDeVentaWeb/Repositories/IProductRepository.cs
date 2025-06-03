public interface IProductRepository
{
    Task removeStockAsync(int productId, int quantity);
    Task addStockAsync(int productId, int quantity);
    Task SaveAsync();
}