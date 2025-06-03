public interface IProductService
{
    Task removeStockAsync(int productId, int quantity);
    Task addStockAsync(int productId, int quantity);
}