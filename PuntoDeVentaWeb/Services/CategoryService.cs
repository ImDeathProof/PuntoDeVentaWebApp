using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using PuntoDeVentaWeb.Models;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }
    public async Task AddCategoryAsync(Category category)
    {
        try
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }
            if (_repository.CategoryExistsAsync(category.Name).Result)
            {
                throw new InvalidOperationException("Category already exists");
            }
            await _repository.AddCategoryAsync(category);
        }
        catch (ArgumentNullException)
        {
            throw new CategoryServiceException("Category cannot be null");
        }
        catch (InvalidOperationException)
        {
            throw new CategoryServiceException("Category already exists");
        }
        catch (DbUpdateException dbEx)
        {
            throw new CategoryServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new CategoryServiceException("An error occurred while adding the category", ex);
        }
    }

    public async Task<bool> CategoryExistsAsync(string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Category name cannot be null or empty", nameof(name));
            }
            return await _repository.CategoryExistsAsync(name);
        }
        catch (ArgumentException)
        {
            throw new CategoryServiceException("Category name cannot be null or empty");
        }
        catch (Exception ex)
        {
            throw new CategoryServiceException("An error occurred while checking if the category exists", ex);
        }
    }

    public async Task<bool> CategoryExistsAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Category ID must be greater than zero");
            }
            return await _repository.CategoryExistsAsync(id);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new CategoryServiceException("Category ID must be greater than zero");
        }
        catch (Exception ex)
        {
            throw new CategoryServiceException("An error occurred while checking if the category exists", ex);
        }
    }

    public async Task DeleteCategoryAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Category ID must be greater than zero");
            }
            var category = await _repository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            await _repository.DeleteCategoryAsync(category);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new CategoryServiceException("Category ID must be greater than zero");
        }
        catch (KeyNotFoundException knfEx)
        {
            throw new CategoryServiceException(knfEx.Message);
        }
        catch (DbUpdateException dbEx)
        {
            throw new CategoryServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new CategoryServiceException("An error occurred while deleting the category", ex);
        }
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        try
        {
            return await _repository.GetAllCategoriesAsync() ?? throw new CategoryServiceException("Failed to retrieve categories. Maybe there are no categories available.");
        }
        catch(DbException dbEx)
        {
            throw new CategoryServiceException("An error occurred while retrieving categories from the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new CategoryServiceException("An error occurred while retrieving all categories", ex);
        }
    }

    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Category ID must be greater than zero");
            }
            return await _repository.GetCategoryByIdAsync(id) ?? throw new CategoryServiceException("Failed to retrieve categories. Maybe there are no categories available.");
        }
        catch(DbException dbEx)
        {
            throw new CategoryServiceException("An error occurred while retrieving categories from the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new CategoryServiceException("An error occurred while retrieving all categories", ex);
        }
    }

    public async Task UpdateCategoryAsync(Category category)
    {
        try
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }
            if (category.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(category.Id), "Category ID must be greater than zero");
            }
            if (await _repository.CategoryExistsAsync(category.Id) == false)
            {
                throw new KeyNotFoundException($"Category with ID {category.Id} not found.");
            }
            await _repository.UpdateCategoryAsync(category);
        }
        catch (ArgumentNullException)
        {
            throw new CategoryServiceException("Category cannot be null");
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new CategoryServiceException("Category ID must be greater than zero");
        }
        catch (KeyNotFoundException knfEx)
        {
            throw new CategoryServiceException(knfEx.Message);
        }
        catch (DbUpdateException dbEx)
        {
            throw new CategoryServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new CategoryServiceException("An error occurred while updating the category", ex);
        }
    }
}
public class CategoryServiceException : Exception
{
    public CategoryServiceException(string message) : base(message) { }
    public CategoryServiceException(string message, Exception innerException) : base(message, innerException) { }
}