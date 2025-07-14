using Microsoft.EntityFrameworkCore;
using PuntoDeVentaWeb.Models;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _repository;
    public BrandService(IBrandRepository repository)
    {
        _repository = repository;
    }
    public async Task AddBrandAsync(Brand brand)
    {
        if (brand == null)
        {
            throw new BrandServiceException("Brand cannot be null");
        }
        try
        {
            // Check if the brand already exists
            bool exists = await _repository.BrandExistsAsync(brand);
            if (exists)
            {
                throw new BrandServiceException("Brand already exists");
            }
            // Add the brand to the database
            await _repository.AddBrandAsync(brand);
        }
        catch (BrandServiceException)
        {
            throw;
        }
    }

    public async Task DeleteBrandAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid brand ID");
        }
        try
        {
            // Delete the brand from the database
            var brand = await _repository.GetBrandByIdAsync(id);
            if (brand == null)
            {
                throw new KeyNotFoundException("Brand not found with the ID: " + id);
            }
            await _repository.DeleteBrandAsync(brand);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (DbUpdateException dbEx)
        {
            throw new BrandServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new BrandServiceException("An error occurred while deleting the brand", ex);
        }
    }

    public Task<IEnumerable<Brand>> GetAllBrandsAsync()
    {
        return _repository.GetAllBrandsAsync();
    }

    public Task<Brand> GetBrandByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid brand ID");
        }
        try
        {
            // Retrieve the brand by ID
            return _repository.GetBrandByIdAsync(id);
        }
        catch (ArgumentException)
        {
            throw new BrandServiceException("Brand not found with the ID: " + id);
        }
        catch (Exception ex)
        {
            throw new BrandServiceException("An error occurred while retrieving the brand", ex);
        }
    }

    public Task UpdateBrandAsync(Brand brand)
    {
        if (brand == null)
        {
            throw new KeyNotFoundException("Brand cannot be null");
        }
        try
        {
            // Check if the brand exists
            bool exists = _repository.BrandExistsAsync(brand).Result;
            if (!exists)
            {
                throw new KeyNotFoundException("Brand not found with the ID: " + brand.Id);
            }
            // Update the brand in the database
            return _repository.UpdateBrandAsync(brand);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (DbUpdateException dbEx)
        {
            throw new BrandServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new BrandServiceException("An error occurred while updating the brand", ex);
        }
    }
}
public class BrandServiceException : Exception
{
    public BrandServiceException(string message) : base(message) { }

    public BrandServiceException(string message, Exception innerException) : base(message, innerException) { }
}