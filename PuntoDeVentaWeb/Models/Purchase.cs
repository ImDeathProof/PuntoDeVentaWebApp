using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoDeVentaWeb.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
        //Supplier
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        //User  
        public int UserId { get; set; }
        public User User { get; set; }
        //Payment
        public int PaymentId { get; set; }
        public Payment Payment { get; set; }

        //PurchaseDetail
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; }
    }
}