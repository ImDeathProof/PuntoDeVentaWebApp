
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
    public async Task UpdateSaleTotalAsync(int saleId, decimal total)
    {
        if (saleId <= 0)
        {
            throw new ArgumentException("Sale ID must be greater than zero", nameof(saleId));
        }
        _context.Sales
        .Where(s => s.Id == saleId)
        .ExecuteUpdate(s => s.SetProperty(s => s.Total, total));
        await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<SaleDetail>> GetSaleDetailsAsync(int saleId)
    {
        if (saleId <= 0)
        {
            throw new ArgumentException("Sale ID must be greater than zero", nameof(saleId));
        }
        return await _context.SaleDetails
            .Where(sd => sd.SaleId == saleId)
            .Include(p => p.Product)
            .ToListAsync();
    }
    public async Task DeleteSaleDetailsAsync(List<SaleDetail> details)
    {
        _context.SaleDetails.RemoveRange(details);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSaleAsync(Sale sale)
    {
        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync();
    }

    public decimal CalculateTotal(List<SaleDetail> details)
    {
        return details.Sum(sd => sd.Quantity * sd.Product.Price);
    }

    public async Task AddSaleAsync(Sale sale)
    {
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();
    }

    public async Task<Sale> GetSaleAsync(int saleId)
    {
        if (saleId <= 0)
        {
            throw new ArgumentException("Sale ID must be greater than zero", nameof(saleId));
        }
        return await _context.Sales
            .FirstOrDefaultAsync(s => s.Id == saleId);
    }

    public async Task<IEnumerable<Sale>> GetSalesAsync()
    {
        return await _context.Sales.ToListAsync();
    }

    public async Task<SaleDetail> GetSaleDetailAsync(int saleId, int detailId)
    {
        if (saleId <= 0 || detailId <= 0)
        {
            throw new ArgumentException("Sale ID and Detail ID must be greater than zero");
        }
        return await _context.SaleDetails
            .Where(sd => sd.SaleId == saleId && sd.Id == detailId)
            .Include(p => p.Product)
            .FirstOrDefaultAsync();
    }

    public async Task AddSaleDetailAsync(SaleDetail saleDetail)
    {
        _context.SaleDetails.Add(saleDetail);
        await _context.SaveChangesAsync();
    }

    public async Task AddSaleDetailsAsync(List<SaleDetail> saleDetails)
    {
        _context.SaleDetails.AddRange(saleDetails);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSaleDetailAsync(SaleDetail saleDetail)
    {
        _context.SaleDetails.Remove(saleDetail);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteVinculedDetailsAsync(int saleId)
    {
        if (saleId <= 0)
        {
            throw new ArgumentException("Sale ID must be greater than zero", nameof(saleId));
        }
        var details = _context.SaleDetails.Where(sd => sd.SaleId == saleId);
        _context.SaleDetails.RemoveRange(details);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSaleDetailAsync(SaleDetail saleDetail)
    {
        _context.SaleDetails.Update(saleDetail);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSaleDetailsAsync(List<SaleDetail> saleDetails)
    {
        _context.SaleDetails.UpdateRange(saleDetails);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CheckIfProductExistsInSaleAsync(int productId, int saleId)
    {
        if (productId <= 0 || saleId <= 0)
        {
            throw new ArgumentException("Invalid Id values provided.");
        }
        return await _context.SaleDetails
            .AnyAsync(sd => sd.ProductId == productId && sd.SaleId == saleId);
    }

    public async Task<SaleDetail> GetSaleDetailByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Sale Detail ID must be greater than zero", nameof(id));
        }
        return await _context.SaleDetails
            .Include(sd => sd.Product)
            .FirstOrDefaultAsync(sd => sd.Id == id);
    }
}