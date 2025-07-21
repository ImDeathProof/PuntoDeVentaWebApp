
using Microsoft.EntityFrameworkCore;
using PuntoDeVentaWeb.Models;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _repository;
    public SaleService(ISaleRepository repository)
    {
        _repository = repository;
    }

    public async Task AddSaleAsync(Sale sale)
    {
        try
        {
            if (sale == null)
            {
                throw new ArgumentNullException(nameof(sale), "Sale cannot be null");
            }
            await _repository.AddSaleAsync(sale);
        }
        catch (ArgumentException ex)
        {
            throw new SaleServiceException("Invalid sale data", ex);
        }
        catch (InvalidOperationException ex)
        {
            throw new SaleServiceException("An error occurred while processing the sale", ex);
        }
        catch (DbUpdateException dbEx)
        {
            throw new SaleServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new SaleServiceException("An error occurred while adding the sale", ex);
        }
    }

    public async Task AddSaleDetailAsync(SaleDetail saleDetail)
    {
        try
        {
            if (saleDetail == null)
            {
                throw new ArgumentNullException(nameof(saleDetail), "Sale detail cannot be null");
            }
            if (saleDetail.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero", nameof(saleDetail.Quantity));
            }
            await _repository.AddSaleDetailAsync(saleDetail);
            await UpdateSaleTotalAsync(saleDetail.SaleId);
        }
        catch (ArgumentNullException ex)
        {
            throw new SaleServiceException("Sale detail cannot be null", ex);
        }
        catch (ArgumentException ex)
        {
            throw new SaleServiceException("Invalid sale detail data", ex);
        }
        catch (DbUpdateException dbEx)
        {
            throw new SaleServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new SaleServiceException("An error occurred while adding the sale detail", ex);
        }
    }

    public async Task AddSaleDetailsAsync(List<SaleDetail> saleDetails)
    {
        try
        {
            if (saleDetails == null || !saleDetails.Any())
            {
                throw new ArgumentNullException(nameof(saleDetails), "Sale details cannot be null or empty");
            }
            foreach (var detail in saleDetails)
            {
                if (detail.Quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than zero", nameof(detail.Quantity));
                }
            }
            await _repository.AddSaleDetailsAsync(saleDetails);
            int saleId = saleDetails.First().SaleId;
            await UpdateSaleTotalAsync(saleId);
        }
        catch (ArgumentNullException ex)
        {
            throw new SaleServiceException("Sale details cannot be null or empty", ex);
        }
        catch (ArgumentException ex)
        {
            throw new SaleServiceException("Invalid sale detail data", ex);
        }
        catch (DbUpdateException dbEx)
        {
            throw new SaleServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new SaleServiceException("An error occurred while adding the sale details", ex);
        }
    }

    public Task<bool> CheckIfProductExistsInSaleAsync(int productId, int saleId)
    {
        try
        {
            if (productId <= 0 || saleId <= 0)
            {
                throw new ArgumentException("Product ID and Sale ID must be greater than zero");
            }
            return _repository.CheckIfProductExistsInSaleAsync(productId, saleId);
        }
        catch (ArgumentException ex)
        {
            throw new SaleServiceException("Invalid product or sale ID", ex);
        }
        catch (DbUpdateException dbEx)
        {
            throw new SaleServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new SaleServiceException("An error occurred while checking if the product exists in the sale", ex);
        }
    }

    public async Task DeleteSaleAsync(Sale sale)
    {

        try
        {
            if (sale == null)
            {
                throw new ArgumentNullException(nameof(sale), "Sale cannot be null");
            }
            await _repository.DeleteVinculedDetailsAsync(sale.Id);
            
            await _repository.DeleteSaleAsync(sale);
        }
        catch (ArgumentNullException ex)
        {
            throw new SaleServiceException("Sale cannot be null", ex);
        }
        catch (DbUpdateException dbEx)
        {
            throw new SaleServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new SaleServiceException("An error occurred while deleting the sale", ex);
        }
    }

    public async Task DeleteSaleDetailAsync(SaleDetail saleDetail)
    {
        try
        {
            if (saleDetail == null)
            {
                throw new ArgumentNullException(nameof(saleDetail), "Sale detail cannot be null");
            }
            await _repository.DeleteSaleDetailAsync(saleDetail);
            await UpdateSaleTotalAsync(saleDetail.SaleId);
        }
        catch (ArgumentNullException ex)
        {
            throw new SaleServiceException("Sale detail cannot be null", ex);
        }
        catch (DbUpdateException dbEx)
        {
            throw new SaleServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new SaleServiceException("An error occurred while deleting the sale detail", ex);
        }
    }

    public async Task DeleteSaleDetailsAsync(List<SaleDetail> details)
    {
        try
        {
            if (details == null || !details.Any())
            {
                throw new ArgumentNullException(nameof(details), "Sale details cannot be null or empty");
            }
            await _repository.DeleteSaleDetailsAsync(details);
            int saleId = details.First().SaleId;
            await UpdateSaleTotalAsync(saleId);
        }
        catch (ArgumentNullException ex)
        {
            throw new SaleServiceException("Sale details cannot be null or empty", ex);
        }
        catch (DbUpdateException dbEx)
        {
            throw new SaleServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new SaleServiceException("An error occurred while deleting the sale details", ex);
        }
    }

    public async Task DeleteVinculedDetailsAsync(int saleId)
    {
        try
        {
            //Obtain the details of the sale
            var detailsEnumerable = await _repository.GetSaleDetailsAsync(saleId);
            var details = detailsEnumerable.ToList();
            if (!details.Any())
            {
                throw new ArgumentException("No sale details found for the given sale ID", nameof(saleId));
            }
            //Delete the details
            await _repository.DeleteSaleDetailsAsync(details);
        }
        catch (ArgumentException ex)
        {
            throw new SaleServiceException("No sale details found for the given sale ID", ex);
        }
        catch (DbUpdateException dbEx)
        {
            throw new SaleServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new SaleServiceException("An error occurred while deleting the vinculed details", ex);
        }
    }

    public async Task<Sale> GetSaleByIdAsync(int saleId)
    {
        try
        {
            if (saleId <= 0)
            {
                throw new ArgumentException("Sale ID must be greater than zero", nameof(saleId));
            }
            return await _repository.GetSaleAsync(saleId);
        }
        catch (ArgumentException ex)
        {
            throw new SaleServiceException("Invalid sale ID", ex);
        }
        catch (DbUpdateException dbEx)
        {
            throw new SaleServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new SaleServiceException("An error occurred while retrieving the sale", ex);
        }
    }

    public async Task<SaleDetail> GetSaleDetailAsync(int saleId, int detailId)
    {
        try
        {
            if (saleId <= 0 || detailId <= 0)
            {
                throw new ArgumentException("Sale ID and Detail ID must be greater than zero");
            }
            return await _repository.GetSaleDetailAsync(saleId, detailId);
        }
        catch (ArgumentException ex)
        {
            throw new SaleServiceException("Invalid sale or detail ID", ex);
        }
        catch (DbUpdateException dbEx)
        {
            throw new SaleServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new SaleServiceException("An error occurred while retrieving the sale detail", ex);
        }
    }

    public async Task<SaleDetail> GetSaleDetailByIdAsync(int id)
    {
        try
        {
            if (id <= 0)
            {
                throw new ArgumentException("Detail ID must be greater than zero", nameof(id));
            }
            return await _repository.GetSaleDetailByIdAsync(id);
        }
        catch (ArgumentException ex)
        {
            throw new SaleServiceException("Invalid detail ID", ex);
        }
        catch (DbUpdateException dbEx)
        {
            throw new SaleServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new SaleServiceException("An error occurred while retrieving the sale detail by ID", ex);
        }
    }

    public async Task<IEnumerable<SaleDetail>> GetSaleDetailsAsync(int saleId)
    {
        try
        {
            if (saleId <= 0)
            {
                throw new ArgumentException("Sale ID must be greater than zero", nameof(saleId));
            }
            return await _repository.GetSaleDetailsAsync(saleId);
        }
        catch (ArgumentException ex)
        {
            throw new SaleServiceException("Invalid sale ID", ex);
        }
        catch (DbUpdateException dbEx)
        {
            throw new SaleServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new SaleServiceException("An error occurred while retrieving the sale details", ex);
        }
    }

    public Task<IEnumerable<Sale>> GetSalesAsync()
    {
        try
        {
            return _repository.GetSalesAsync();
        }
        catch (DbUpdateException dbEx)
        {
            throw new SaleServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new SaleServiceException("An error occurred while retrieving the sales", ex);
        }
    }

    public async Task UpdateSaleDetailAsync(SaleDetail saleDetail)
    {
        try
        {
            if (saleDetail == null)
            {
                throw new ArgumentNullException(nameof(saleDetail), "Sale detail cannot be null");
            }
            if (saleDetail.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero", nameof(saleDetail.Quantity));
            }
            await _repository.UpdateSaleDetailAsync(saleDetail);
            await UpdateSaleTotalAsync(saleDetail.SaleId);
        }
        catch (ArgumentNullException ex)
        {
            throw new SaleServiceException("Sale detail cannot be null", ex);
        }
        catch (ArgumentException ex)
        {
            throw new SaleServiceException("Invalid sale detail data", ex);
        }
        catch (DbUpdateException dbEx)
        {
            throw new SaleServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new SaleServiceException("An error occurred while updating the sale detail", ex);
        }
    }

    public async Task UpdateSaleDetailsAsync(List<SaleDetail> saleDetails)
    {
        try
        {
            if (saleDetails == null || !saleDetails.Any())
            {
                throw new ArgumentNullException(nameof(saleDetails), "Sale detail cannot be null");
            }
            foreach (var saleDetail in saleDetails)
            {
                if (saleDetail.Quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than zero", nameof(saleDetail.Quantity));
                }
            }
            await _repository.UpdateSaleDetailsAsync(saleDetails);
            int saleId = saleDetails.First().SaleId;
            await UpdateSaleTotalAsync(saleId);
        }
        catch (ArgumentNullException ex)
        {
            throw new SaleServiceException("Sale detail cannot be null", ex);
        }
        catch (ArgumentException ex)
        {
            throw new SaleServiceException("Invalid sale detail data", ex);
        }
        catch (DbUpdateException dbEx)
        {
            throw new SaleServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new SaleServiceException("An error occurred while updating the sale detail", ex);
        }
    }

    public async Task UpdateSaleTotalAsync(int saleId)
    {
        try
        {
            var saledetailsEnumerable = await _repository.GetSaleDetailsAsync(saleId);
            var saledetails = saledetailsEnumerable.ToList();
            if (!saledetails.Any())
            {
                return;
            }
            decimal total = _repository.CalculateTotal(saledetails);
            //Update the total of the sale
            await _repository.UpdateSaleTotalAsync(saleId, total);
        }
        catch (ArgumentException ex)
        {
            throw new SaleServiceException("Invalid sale ID", ex);
        }
        catch (DbUpdateException dbEx)
        {
            throw new SaleServiceException("An error occurred while updating the database", dbEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating sale total: {ex.Message}");
        }
    }
}
public class SaleServiceException : Exception
{
    public SaleServiceException(string message) : base(message) { }
    public SaleServiceException(string message, Exception innerException) : base(message, innerException) { }
}
    
