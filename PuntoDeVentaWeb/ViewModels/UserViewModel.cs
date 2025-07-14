using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoDeVentaWeb.Models
{
    public class UserViewModel
    {
        public required string Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [Display(Name = "Name")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "El apellido es requerido")]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "User role is requiered")]
        [Display(Name = "User role")]
        public required string UserRole { get; set; }

        [Required(ErrorMessage = "Phone number is requiered")]
        [Display(Name = "Phone number")]
        public required string PhoneNumber { get; set; }

        [Display(Name = "State")]
        public bool IsActive { get; set; }

    }
}