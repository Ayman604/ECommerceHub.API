using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Presistence.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Presistence.Data.DbContexts
{
    public class StoreDbConetxt : DbContext
    {
        public StoreDbConetxt(DbContextOptions<StoreDbConetxt> options): base(options) { }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
           // modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfigurations).Assembly);
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
    }
}
