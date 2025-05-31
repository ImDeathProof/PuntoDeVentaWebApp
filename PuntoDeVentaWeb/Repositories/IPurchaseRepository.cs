
public interface IPurchaseRepository
{
    Task updateTotalAsync(int purchaseId);
    Task SaveAsync();
}