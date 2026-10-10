using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Presistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerce.Presistence.Data.DataSeed
{
    public class DataInitializer : IDataInitializer
    {
        private readonly StoreDbConetxt _context;
        public DataInitializer(StoreDbConetxt context)
        {
            _context = context;
        }
        public async Task InitializeAsync()
        {

            try
            {
                var HasProducts = await _context.Products.AnyAsync();
                var HasBrands = await _context.ProductBrands.AnyAsync();
                var HasTypes = await _context.ProductTypes.AnyAsync();

                if (HasProducts && HasBrands && HasTypes) return;
               
                if (!HasBrands)
                {
                   await SeedDataFromJson<ProductBrand, int>(fileNmae: "brands.json", _context.ProductBrands);
                }
                if (!HasTypes)
                {
                    await SeedDataFromJson<ProductType, int>(fileNmae: "types.json", _context.ProductTypes);

                }
               await _context.SaveChangesAsync();
                if (!HasProducts)
                {
                    await SeedDataFromJson<Product, int>(fileNmae: "productss.json", _context.Products);
                    await _context.SaveChangesAsync();
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while initializing the database: {ex.Message}");
            }

        }

        private async Task SeedDataFromJson<T, TKey>(string fileNmae, DbSet<T> dbset) where T : BaseEntity<TKey>
        {
            var FilePath = @"..\Ecommerce.Presistence\Data\DataSeed\JsonFiles\" + fileNmae;
            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException($"The file {fileNmae} was not found in the path {FilePath}");
            }

            try
            {
                #region OLD Way 
                //var jsonData = File.ReadAllText(FilePath);
                //  var entities = System.Text.Json.JsonSerializer.Deserialize<List<T>>(jsonData);

                #endregion
                var dataStream = File.OpenRead(FilePath); // Read From File
                var data =await JsonSerializer
                            .DeserializeAsync<List<T>>(dataStream,
                            options: new JsonSerializerOptions
                { PropertyNameCaseInsensitive = true });

                if (data is not null)
                {
                   await dbset.AddRangeAsync(data); // Add Locally in DbSet
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while seeding data from {fileNmae}: {ex.Message}");
            }
        }  
    }
}
  