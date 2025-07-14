using PuntoDeVentaWeb.Models;

public interface IBrandService
{
    Task<Brand> GetBrandByIdAsync(int id);
    Task<IEnumerable<Brand>> GetAllBrandsAsync();
    Task AddBrandAsync(Brand brand);
    Task UpdateBrandAsync(Brand brand);
    Task DeleteBrandAsync(int id);
}