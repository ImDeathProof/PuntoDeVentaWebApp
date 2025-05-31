public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _repository;

    public PurchaseService(IPurchaseRepository repository)
    {
        _repository = repository;
    }

    public async Task UpdatePurchaseTotalAsync(int purchaseId)
    {
        await _repository.updateTotalAsync(purchaseId);
        await _repository.SaveAsync();
    }
}