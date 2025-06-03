using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoDeVentaWeb.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Display(Name = "Product Name")]
        [Required(ErrorMessage = "The product name is required.")]
        public string Name { get; set; }
        [Display(Name = "Description")]
        [Required(ErrorMessage = "The product description is required.")]
        public string Description { get; set; }
        [Display(Name = "Category")]
        [Required(ErrorMessage = "The category is required.")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        [Display(Name = "Brand")]
        [Required(ErrorMessage = "The brand is required.")]
        public int BrandId { get; set; }
        public Brand? Brand { get; set; }
        [Display(Name = "SKU")]
        [Required(ErrorMessage = "The SKU is required.")]
        public int SKU { get; set; }
        [Display(Name = "Stock")]
        [Required(ErrorMessage = "The stock is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative number.")]
        public int Stock { get; set; }
        [Display(Name = "Price")]
        [Required(ErrorMessage = "The price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal Price { get; set; }
        [Display(Name = "Image URL")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string? Image { get; set; }

    }
}