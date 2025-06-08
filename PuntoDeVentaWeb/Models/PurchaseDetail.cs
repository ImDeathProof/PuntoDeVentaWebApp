using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Display(Name = "Product")]
        [Required(ErrorMessage = "The product is required.")]
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

    }
}