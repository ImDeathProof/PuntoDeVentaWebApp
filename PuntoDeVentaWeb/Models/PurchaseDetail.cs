using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoDeVentaWeb.Models
{
    public class PurchaseDetail
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        //Purchase
        [Required]
        [Display(Name = "Purchase ID")]
        public int PurchaseId { get; set; }
        public Purchase? Purchase { get; set; }
        //Product
        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

    }
}