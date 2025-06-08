public interface IProductRepository
{
    Task removeStockAsync(int productId, int quantity);
    Task addStockAsync(int productId, int quantity);
    bool checkIfProductExistsInPurchaseAsync(int productId);
    bool checkIfProductExistsInSaleAsync(int productId);
    Task SaveAsync();
}