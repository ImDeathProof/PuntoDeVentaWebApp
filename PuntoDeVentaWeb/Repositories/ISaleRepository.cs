public interface ISaleRepository
{
    Task UpdateSaleTotalAsync(int saleId);
    Task SaveAsync();
}