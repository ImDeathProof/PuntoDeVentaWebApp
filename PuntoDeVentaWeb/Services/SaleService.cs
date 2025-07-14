
using PuntoDeVentaWeb.Models;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _repository;
    public SaleService(ISaleRepository repository)
    {
        _repository = repository;
    }

    public async Task DeleteSaleAsync(Sale sale)
    {
        if (sale == null)
        {
            throw new ArgumentNullException(nameof(sale), "Sale cannot be null");
        }

        try
        {
            //Delete the sale
            _repository.DeleteSaleAsync(sale);
            await _repository.SaveAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting sale: {ex.Message}");
        }
    }

    public async Task DeleteVinculedDetailsAsync(int saleId)
    {
        //Obtain the details of the sale
        var details = await _repository.GetSaleDetailsAsync(saleId);
        if (!details.Any())
        {
            return;
        }
        try
        {
            //Delete the details
            _repository.DeleteSaleDetailsAsync(details);
            await _repository.SaveAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting sale details: {ex.Message}");
        }
    }

    public async Task UpdateSaleTotalAsync(int saleId)
    {
        var saledetails = await _repository.GetSaleDetailsAsync(saleId);
        if (!saledetails.Any())
        {
            return;
        }
        try
        {
            decimal total = _repository.CalculateTotal(saledetails);
            //Update the total of the sale
            _repository.UpdateSaleTotalAsync(saleId, total);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating sale total: {ex.Message}");
        }
    }
}
