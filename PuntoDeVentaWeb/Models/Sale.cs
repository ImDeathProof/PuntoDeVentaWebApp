using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoDeVentaWeb.Models
{
    public class Sale
    {
        public int Id { get; set; }
        [Display(Name = "Client")]
        [Required(ErrorMessage = "The client is required.")]
        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        [Display(Name = "Total")]
        public decimal Total { get; set; }

        [Display(Name = "Date")]
        [Required(ErrorMessage = "The date is required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(Name = "User")]
        [Required(ErrorMessage = "The seller is required.")]
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    
        [Display(Name = "Status")]
        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public Status? Status { get; set; }

        [Display(Name = "Payment Method")]
        [Required]
        public int PaymentMethodId { get; set; }
        [ForeignKey("PaymentMethodId")]
        public PaymentMethod? PaymentMethod { get; set; }

        public ICollection<SaleDetail> SaleDetails { get; set; } = new List<SaleDetail>();
    }
}