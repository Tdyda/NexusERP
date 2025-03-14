using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NexusERP.Enums;
using NexusERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace NexusERP.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<MaterialRequestModel> MaterialsRequest { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
                : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Konfiguracja przechowywania enum jako tekst
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v)
                );
        }
    }
}
