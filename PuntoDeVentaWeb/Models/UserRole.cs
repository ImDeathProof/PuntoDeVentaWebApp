using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PuntoDeVentaWeb.Models
{
    public class UserRole : IdentityRole<string>
    {
        public UserRole() : base() 
        {
            Id = Guid.NewGuid().ToString();
        }
        public UserRole(string roleName) : this()
        {
            Name = roleName;
        }
        public int AccessLevel { get; set; } 
        public string? Description { get; set; }
    }
}