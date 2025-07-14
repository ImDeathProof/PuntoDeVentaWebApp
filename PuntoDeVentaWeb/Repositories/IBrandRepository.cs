using PuntoDeVentaWeb.Models;
public interface IBrandRepository
{
    Task<Brand> GetBrandByIdAsync(int id);
    Task<bool> BrandExistsAsync(Brand brand);
    Task<IEnumerable<Brand>> GetAllBrandsAsync();
    Task AddBrandAsync(Brand brand);
    Task UpdateBrandAsync(Brand brand);
    Task DeleteBrandAsync(Brand brand);
    Task SaveAsync();
}