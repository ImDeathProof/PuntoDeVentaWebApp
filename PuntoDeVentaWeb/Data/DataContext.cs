using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PuntoDeVentaWeb.Models;

namespace PuntoDeVentaWeb.Data
{
    public class DataContext : IdentityDbContext<User, UserRole, string>
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
        public DbSet<Models.PurchaseDetail> PurchaseDetails { get; set; }
        public DbSet<Models.Sale> Sales { get; set; }
        public DbSet<Models.SaleDetail> SaleDetails { get; set; }
        public DbSet<Models.Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Relationships and constraints for Product entity
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany()
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            // Relationships and constraints for Purchase entity
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.Supplier)
                .WithMany()
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Purchase>()
                .HasOne(p => p.PaymentMethod)
                .WithMany()
                .HasForeignKey(p => p.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
            // Relationships and constraints for purchase detail entity
            modelBuilder.Entity<PurchaseDetail>()
                .HasOne(pd => pd.Product)
                .WithMany()
                .HasForeignKey(pd => pd.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            // Relationships and constraints for Sale entity
            modelBuilder.Entity<Sale>()
                .HasOne(s => s.Client)
                .WithMany()
                .HasForeignKey(s => s.ClientId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SaleDetail>()
            .HasOne(sd => sd.Product)
                .WithMany()
                .HasForeignKey(sd => sd.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            // Relationships and constraints for Payment entity
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.PaymentMethod)
                .WithMany()
                .HasForeignKey(p => p.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}