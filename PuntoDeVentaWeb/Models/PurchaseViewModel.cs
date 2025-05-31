using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoDeVentaWeb.Models
{
    public class PurchaseViewModel
    {
        [Required]
        public Purchase Purchase { get; set; } = new Purchase();

        [Required]
        [Display(Name = "Purchase Details")]
        public List<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();
    }
}