using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoDeVentaWeb.Models
{
    public class PurchaseViewModel
    {
            // Datos de la compra (Purchase)
    required public Purchase Purchase { get; set; }

    // Lista de detalles (PurchaseDetail)
    public List<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
    }
}