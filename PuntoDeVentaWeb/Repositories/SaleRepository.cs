
using Microsoft.EntityFrameworkCore;
using PuntoDeVentaWeb.Data;

public class SaleRepository : ISaleRepository
{
    private readonly DataContext _context;
    public SaleRepository(DataContext context)
    {
        _context = context;
    }
    public async Task UpdateSaleTotalAsync(int saleId)
    {
        var sale = await _context.Sales.FindAsync(saleId);
        if (sale == null)
        {
            throw new Exception("Sale not found");
        }
        try
        {
            decimal total = 0;
            var saleDetails = await _context.SaleDetails
                .Where(sd => sd.SaleId == saleId)
                .Include(p => p.Product)
                .ToListAsync();
            if (saleDetails != null)
            {
                foreach (var detail in saleDetails)
                {
                    total += detail.Quantity * detail.Product.Price;
                }
            }
            sale.Total = total;
            _context.Sales.Update(sale);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating sale total: {ex.Message}");
        }
    }
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}