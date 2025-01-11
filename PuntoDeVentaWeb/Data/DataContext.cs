using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PuntoDeVentaWeb.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<Models.Brand> Brands { get; set; }
        public DbSet<Models.Category> Categories { get; set; }
        public DbSet<Models.Client> Clients { get; set; }
        public DbSet<Models.Payment> Payments { get; set; }
        public DbSet<Models.PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Models.Product> Products { get; set; }
        public DbSet<Models.Purchase> Purchases { get; set; }
        public DbSet<Models.Sale> Sales { get; set; }
        public DbSet<Models.Supplier> Suppliers { get; set; }
        public DbSet<Models.User> Users { get; set; }
    }
}