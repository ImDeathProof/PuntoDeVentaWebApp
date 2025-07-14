using PuntoDeVentaWeb.Models;

public interface ISaleService
{
    Task UpdateSaleTotalAsync(int saleId);
    Task DeleteVinculedDetailsAsync(int saleId);
    Task DeleteSaleAsync(Sale sale);

}