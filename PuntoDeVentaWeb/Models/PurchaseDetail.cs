using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoDeVentaWeb.Models
{
    public class PurchaseDetail
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        //Purchase
        public int PurchaseId { get; set; }
        public Purchase Purchase { get; set; }
        //Product
        public int ProductId { get; set; }
        public Product? Product { get; set; }

    }
}