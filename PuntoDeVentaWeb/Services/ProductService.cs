

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task addStockAsync(int productId, int quantity)
    {
        await _repository.addStockAsync(productId, quantity);
        await _repository.SaveAsync();
    }

    public async Task removeStockAsync(int productId, int quantity)
    {
        await _repository.removeStockAsync(productId, quantity);
        await _repository.SaveAsync();
    }
}