using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PuntoDeVentaWeb.Models
{
    public class Status
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}