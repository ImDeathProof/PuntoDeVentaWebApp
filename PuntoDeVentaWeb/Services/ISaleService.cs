using PuntoDeVentaWeb.Models;

public interface ISaleService
{

    //Sales
    Task AddSaleAsync(Sale sale);
    Task<Sale> GetSaleByIdAsync(int saleId);
    Task<IEnumerable<Sale>> GetSalesAsync();
    Task DeleteSaleAsync(Sale sale);
    Task UpdateSaleTotalAsync(int saleId);

    //Saledetails
    Task<IEnumerable<SaleDetail>> GetSaleDetailsAsync(int saleId);
    Task<SaleDetail> GetSaleDetailAsync(int saleId, int detailId);
    Task<SaleDetail> GetSaleDetailByIdAsync(int id);
    Task AddSaleDetailAsync(SaleDetail saleDetail);
    Task AddSaleDetailsAsync(List<SaleDetail> saleDetails);
    Task DeleteSaleDetailsAsync(List<SaleDetail> details);
    Task DeleteSaleDetailAsync(SaleDetail saleDetail);
    Task DeleteVinculedDetailsAsync(int saleId);
    Task UpdateSaleDetailAsync(SaleDetail saleDetail);
    Task UpdateSaleDetailsAsync(List<SaleDetail> saleDetails);
    Task<bool> CheckIfProductExistsInSaleAsync(int productId, int saleId);

}