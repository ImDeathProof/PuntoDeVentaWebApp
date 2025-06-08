using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoDeVentaWeb.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client? Client { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }

        public ICollection<SaleDetail> SaleDetails { get; set; }
    }
}