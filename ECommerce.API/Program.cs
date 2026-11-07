using ECommerce.API.CustomMiddlewares;
using ECommerce.API.Extensions;
using ECommerce.Domain.Contracts;
using ECommerce.Presentation.Controllers;
using ECommerce.Presistence.Data.DataSeed;
using ECommerce.Presistence.Data.DbContexts;
using ECommerce.Presistence.Repositories;
using ECommerce.Services;
using ECommerce.Services.Abstraction;
using ECommerce.Services.MappingProfiles;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);
#region RegistrationServices 

// Add services to the container.

builder.Services.AddControllers()
    .AddApplicationPart(typeof(ProductsController).Assembly);
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

builder.Services.AddTransient<ProductPictureUrlResolver>(); // This is not needed because we are using AutoMapper's DI to resolve the dependencies of the resolver

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")!);
});

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddScoped<IBasketService, BasketService>();

builder.Services.AddScoped<ICacheRepositiory, CacheRepository>();
builder.Services.AddScoped<ICacheService, CacheService>();
#endregion
var app = builder.Build();


await app.MigrateDataBaseAsync();
//await app.MigrateIdentityDataBaseAsync(); //Later

await app.SeedDataAsync();
//await app.SeedIdentityDataAsync(); // Later

#region Pipeline 
app.UseMiddleware<ExceptionHandlerMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DisplayRequestDuration();
        options.EnableFilter();
        options.DocExpansion(DocExpansion.None);
    });
}
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

#endregion
await app.RunAsync();
