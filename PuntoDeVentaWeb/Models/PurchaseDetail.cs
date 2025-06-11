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
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
        //Purchase
        [Required]
        [Display(Name = "Purchase ID")]
        public int PurchaseId { get; set; }
        public Purchase? Purchase { get; set; }
        //Product
        [Display(Name = "Product")]
        [Required]
        public int ProductId { get; set; }
        [Display(Name = "Product")]
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

    }
}