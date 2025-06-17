using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoDeVentaWeb.Models
{
    public class SalesViewModel
    {
        [Required]
        public Sale Sale { get; set; } = new Sale();

        [Required]
        [Display(Name = "Sale Details")]
        public List<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    }
}