using Microsoft.AspNetCore.Mvc.Rendering;
using PuntoDeVentaWeb.Models;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category> GetCategoryByIdAsync(int id);
    Task AddCategoryAsync(Category category);
    Task UpdateCategoryAsync(Category category);
    Task DeleteCategoryAsync(Category category);
    Task<bool> CategoryExistsAsync(string name);
    Task<bool> CategoryExistsAsync(int id);
}
