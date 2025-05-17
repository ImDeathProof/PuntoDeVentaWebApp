using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PuntoDeVentaWeb.Models
{
    public class User : IdentityUser
    {
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? UserRoleId { get; set; }
        public UserRole? UserRole { get; set; }
    }
}