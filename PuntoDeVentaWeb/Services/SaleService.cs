
public class SaleService : ISaleService
{
    private readonly ISaleRepository _repository;
    public SaleService(ISaleRepository repository)
    {
        _repository = repository;
    }
    public async Task UpdateSaleTotalAsync(int saleId)
    {
        await _repository.UpdateSaleTotalAsync(saleId);
        await _repository.SaveAsync();
    }
}