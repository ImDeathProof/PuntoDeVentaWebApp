
using Microsoft.EntityFrameworkCore;
using PuntoDeVentaWeb.Data;
using PuntoDeVentaWeb.Models;

public class SaleRepository : ISaleRepository
{
    private readonly DataContext _context;
    public SaleRepository(DataContext context)
    {
        _context = context;
    }
    public void UpdateSaleTotalAsync(int saleId, decimal total)
    {
        _context.Sales
        .Where(s => s.Id == saleId)
        .ExecuteUpdate(s => s.SetProperty(s => s.Total, total));
    }
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
    public async Task<List<SaleDetail>> GetSaleDetailsAsync(int saleId)
    {
        return await _context.SaleDetails
            .Where(sd => sd.SaleId == saleId)
            .Include(p => p.Product)
            .ToListAsync();
    }
    public void DeleteSaleDetailsAsync(List<SaleDetail> details)
    {
        _context.SaleDetails.RemoveRange(details);
    }

    public void DeleteSaleAsync(Sale sale)
    {
        _context.Sales.Remove(sale);
    }

    decimal ISaleRepository.CalculateTotal(List<SaleDetail> details)
    {
        return details.Sum(sd => sd.Quantity * sd.Product.Price);
    }
}