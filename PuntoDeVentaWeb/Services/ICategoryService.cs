using PuntoDeVentaWeb.Models;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category> GetCategoryByIdAsync(int id);
    Task AddCategoryAsync(Category category);
    Task UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(int id);
    Task<bool> CategoryExistsAsync(string name);
    Task<bool> CategoryExistsAsync(int id);
    
}
