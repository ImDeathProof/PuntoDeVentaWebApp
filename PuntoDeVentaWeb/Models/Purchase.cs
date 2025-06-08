using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoDeVentaWeb.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        [Display(Name = "Total")]
        public decimal Total { get; set; }
        [Display(Name = "Datetime")]
        [Required(ErrorMessage = "The date is required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; } = DateTime.Now;

        public int SupplierId { get; set; }
        [Display(Name = "Supplier")]
        [Required(ErrorMessage = "The supplier is required.")]
        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }

        public string UserId { get; set; }
        [Display(Name = "User")]
        [Required(ErrorMessage = "The user is required.")]
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public int PaymentMethodId { get; set; }
        [Display(Name = "Payment Method")]
        [Required(ErrorMessage = "The payment method is required.")]
        [ForeignKey("PaymentMethodId")]    
        public PaymentMethod? PaymentMethod { get; set; }

        //PurchaseDetail
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();

    }
}