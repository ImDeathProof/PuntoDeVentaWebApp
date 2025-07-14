using Microsoft.EntityFrameworkCore;
using PuntoDeVentaWeb.Data;
using PuntoDeVentaWeb.Models;
public class BrandRepository : IBrandRepository
{
    private readonly DataContext _context;

    public BrandRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Brand> GetBrandByIdAsync(int id)
    {
        return await _context.Brands.FindAsync(id);
    }

    public async Task<IEnumerable<Brand>> GetAllBrandsAsync()
    {
        return await _context.Brands.ToListAsync();
    }

    public async Task AddBrandAsync(Brand brand)
    {
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBrandAsync(Brand brand)
    {
        _context.Brands.Update(brand);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBrandAsync(Brand brand)
    {
        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();
        
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> BrandExistsAsync(Brand brand)
    {
        return await _context.Brands.AnyAsync(b => b.Id == brand.Id || b.Name.ToLower() == brand.Name.ToLower());
    }
}