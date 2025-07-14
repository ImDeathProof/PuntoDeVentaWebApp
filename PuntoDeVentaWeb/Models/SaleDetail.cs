using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoDeVentaWeb.Models
{
    public class SaleDetail
    {
        public int Id { get; set; }
        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "The quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "The quantity must be greater than zero.")]
        public int Quantity { get; set; }
        [Display(Name = "Sale")]
        [Required(ErrorMessage = "The sale is required.")]
        public int SaleId { get; set; }
        public Sale? Sale { get; set; }
        [Display(Name = "Product")]
        [Required(ErrorMessage = "The product is required.")]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }
}