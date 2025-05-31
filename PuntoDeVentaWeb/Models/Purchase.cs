using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoDeVentaWeb.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; } = DateTime.Now;
        //Supplier
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        //User  
        public string UserId { get; set; }
        public User? User { get; set; }
        //Payment
        public int PaymentMethodId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }

        //PurchaseDetail
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();

    }
}