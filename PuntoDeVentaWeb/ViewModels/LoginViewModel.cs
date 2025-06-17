using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoDeVentaWeb.Models
{
    public class LoginViewModel
{
    [Required(ErrorMessage = "The email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "The password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Name")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Lastname")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "This field is required")]
    [EmailAddress(ErrorMessage = "Formato de email inválido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "User role")]
    public string SelectedRole { get; set; }

    [Required(ErrorMessage = "This field is required")]
    [Display(Name = "Phone number")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "This field is required")]
    [StringLength(100, ErrorMessage = "The {0} must have at least {2} characters.", MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Passwords must be the same")]
    public string ConfirmPassword { get; set; }
}
}