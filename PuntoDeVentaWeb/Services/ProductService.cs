

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PuntoDeVentaWeb.Models;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task AddProductAsync(Product product)
    {
        try
        {
            if (product == null) throw new ArgumentNullException(nameof(product), "Product cannot be null");
            await _repository.AddProductAsync(product);
        }
        catch (ArgumentNullException ex)
        {
            throw new ArgumentNullException("Product cannot be null", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Database update failed", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while adding the product", ex);
        }
    }

    public async Task addStockAsync(int productId, int quantity)
    {
        try
        {
            if (productId <= 0) throw new ArgumentException("Invalid product ID");
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero");
            var product = await _repository.GetProductByIdAsync(productId);
            if (product == null) throw new KeyNotFoundException("Product not found");
            product.Stock += quantity;
            await _repository.UpdateProductAsync(product);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException("Invalid input", ex);
        }
        catch (KeyNotFoundException ex)
        {
            throw new KeyNotFoundException("Product not found", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Database update failed", ex);
        }
        catch(Exception ex)
        {
            throw new Exception("An error occurred while adding stock", ex);
        }
    }

    public bool checkIfProductExistsInPurchaseAsync(int productId)
    {
        try
        {
            if (productId <= 0) throw new ArgumentException("Invalid product ID");
            return _repository.checkIfProductExistsInPurchaseAsync(productId);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException("Invalid input", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while checking product existence in purchase", ex);
        }
    }

    public bool checkIfProductExistsInSaleAsync(int productId)
    {
        try {
            if (productId <= 0) throw new ArgumentException("Invalid product ID");
            return _repository.checkIfProductExistsInSaleAsync(productId);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException("Invalid input", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while checking product existence in sale", ex);
        }
    }

    public async Task DeleteProductAsync(Product product)
    {
        try
        {
            if (product == null) throw new ArgumentNullException(nameof(product), "Product cannot be null");
            await _repository.DeleteProductAsync(product);
        }
        catch (ArgumentNullException ex)
        {
            throw new ArgumentNullException("Product cannot be null", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Database update failed", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while deleting the product", ex);
        }
    }

    public async Task<Product> GetProductByIdAsync(int productId)
    {
        try
        {
            if (productId <= 0) throw new ArgumentException("Invalid product ID");
            return await _repository.GetProductByIdAsync(productId);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException("Invalid input", ex);
        }
        catch (KeyNotFoundException ex)
        {
            throw new KeyNotFoundException("Product not found", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving the product", ex);
        }
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        try
        {
            return await _repository.GetProductsAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving products", ex);
        }
    }

    public async Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId)
    {
        try
        {
            if (brandId <= 0) throw new ArgumentException("Invalid brand ID");
            return await _repository.GetProductsByBrandAsync(brandId);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException("Invalid input", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving products by brand", ex);
        }
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        try
        {
            if (categoryId <= 0) throw new ArgumentException("Invalid category ID");
            return await _repository.GetProductsByCategoryAsync(categoryId);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException("Invalid input", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving products by category", ex);
        }
    }

    public async Task<SelectList> GetProductSelectListAsync()
    {
        try
        {
            var products = await _repository.GetProductsAsync() ?? Enumerable.Empty<Product>();
            return new SelectList(products, "Id", "Name");
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while retrieving product select list", ex);
        }
    }

    public async Task removeStockAsync(int productId, int quantity)
    {
        try
        {
            if (productId <= 0) throw new ArgumentException("Invalid product ID");
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero");
            var product = await _repository.GetProductByIdAsync(productId);
            if (product == null) throw new KeyNotFoundException("Product not found");
            product.Stock -= quantity;
            await _repository.UpdateProductAsync(product);
        }
        catch (ArgumentException ex)
        {
            throw new ArgumentException("Invalid input", ex);
        }
        catch (KeyNotFoundException ex)
        {
            throw new KeyNotFoundException("Product not found", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Database update failed", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while removing stock", ex);
        }
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await _repository.GetProductsAsync();
            return await _repository.SearchProductsAsync(searchTerm);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while searching for products", ex);
        }
    }

    public async Task UpdateProductAsync(Product product)
    {
        try
        {
            if (product == null) throw new ArgumentNullException(nameof(product), "Product cannot be null");
            await _repository.UpdateProductAsync(product);
        }
        catch (ArgumentNullException ex)
        {
            throw new ArgumentNullException("Product cannot be null", ex);
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("Database update failed", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while updating the product", ex);
        }
    }
}