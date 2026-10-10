using ECommerce.API.Extensions;
using ECommerce.Domain.Contracts;
using ECommerce.Presistence.Data.DataSeed;
using ECommerce.Presistence.Data.DbContexts;
using ECommerce.Presistence.Repositories;
using ECommerce.Services;
using ECommerce.Presentation.Controllers;
using ECommerce.Services.Abstraction;
using ECommerce.Services.MappingProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;

var builder = WebApplication.CreateBuilder(args);
#region RegistrationServices 

// Add services to the container.

builder.Services.AddControllers()
    .AddApplicationPart(typeof(ProductController).Assembly);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StoreDbConetxt>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Befor Configuring Pipeline  You must Call Seed
// You need to Call Data initializer
builder.Services.AddScoped<IDataInitializer, DataInitializer>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(ServiceAssemblyReference).Assembly);
//builder.Services.AddAutoMapper(X => X.AddProfile<ProductProfile>());
//builder.Services.AddTransient<ProductPictureUrlResolver>(); // This is not needed because we are using AutoMapper's DI to resolve the dependencies of the resolver
builder.Services.AddScoped<IProductService, ProductService>();
#endregion
var app = builder.Build();


await app.MigrateDataBaseAsync();
await app.SeedDataAsync();


#region Pipeline 

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#endregion
await app.RunAsync();
 