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

        [Display(Name = "Date")]
        [Required(ErrorMessage = "The date is required.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "The supplier is required.")]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }

        [Display(Name = "User")]
        [Required(ErrorMessage = "The user is required.")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Display(Name = "Payment Method")]
        [Required(ErrorMessage = "The payment method is required.")]
        public int PaymentMethodId { get; set; }

        [ForeignKey("PaymentMethodId")]    
        public PaymentMethod? PaymentMethod { get; set; }

        //PurchaseDetail
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = new List<PurchaseDetail>();

    }
}