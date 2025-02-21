using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NexusERP.Data;
using System;

namespace NexusERP
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Configure your database connection string for design time
            optionsBuilder.UseMySql(
                "Server=10.172.111.78;Database=order_warehouse_db;User=tdyda;Password=Online1234!;",
                new MySqlServerVersion(new Version(10, 11, 6))
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
