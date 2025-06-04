using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoDeVentaWeb.Models
{
    public class ProfileViewModel
{
    
    [Display(Name = "Id")]
    public string? Id { get; set; }
    
    
    [Display(Name = "Name")]
    public string? Name { get; set; }
    
    [Display(Name = "LastName")]
    public string? LastName { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }
    
    [Phone]
    [Display(Name = "Phonenumber")]
    public string? PhoneNumber { get; set; }
}
}