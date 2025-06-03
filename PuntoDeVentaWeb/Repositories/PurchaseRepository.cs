using Microsoft.EntityFrameworkCore;
using PuntoDeVentaWeb.Data;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly DataContext _context;

    public PurchaseRepository(DataContext context)
    {
        _context = context;
    }

    public async Task updateTotalAsync(int purchaseId)
    {
        var purchase = await _context.Purchases.FindAsync(purchaseId);
        if (purchase == null) throw new Exception("Purchase not found");

        decimal total = 0;
        var purchaseDetails = await _context.PurchaseDetails
            .Where(pd => pd.PurchaseId == purchaseId)
            .Include(pd => pd.Product)
            .ToListAsync();
        if (purchaseDetails != null)
        {
            foreach (var detail in purchaseDetails)
            {
                total += detail.Quantity * detail.Product.Price;
            }
        }
        purchase.Total = total;
        _context.Purchases.Update(purchase);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}