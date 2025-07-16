using Microsoft.EntityFrameworkCore;
using PuntoDeVentaWeb.Data;
using PuntoDeVentaWeb.Models;

public class CategoryRepository : ICategoryRepository
{
    private readonly DataContext _context;
    public CategoryRepository(DataContext context)
    {
        _context = context;
    }
    public async Task AddCategoryAsync(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CategoryExistsAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        return false;

        return await _context.Categories
        .AnyAsync(c => EF.Functions.Collate(c.Name, "SQL_Latin1_General_CP1_CI_AI") == name);
    }

    public async Task<bool> CategoryExistsAsync(int id)
    {
        return await _context.Categories.AnyAsync(c => c.Id == id);
    }

    public async Task DeleteCategoryAsync(Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id) ?? throw new KeyNotFoundException($"Category with ID {id} not found.");
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }
}