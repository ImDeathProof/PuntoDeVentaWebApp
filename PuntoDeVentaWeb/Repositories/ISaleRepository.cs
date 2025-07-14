using PuntoDeVentaWeb.Models;

public interface ISaleRepository
{
    Task<List<SaleDetail>> GetSaleDetailsAsync(int saleId);
    decimal CalculateTotal(List<SaleDetail> details);
    void UpdateSaleTotalAsync(int saleId, decimal total);
    void DeleteSaleDetailsAsync(List<SaleDetail> details);
    void DeleteSaleAsync(Sale sale);
    Task SaveAsync();
}