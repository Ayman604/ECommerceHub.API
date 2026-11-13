using ECommerce.API.CustomMiddlewares;
using ECommerce.API.Extensions;
using ECommerce.API.Factories;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.IdentityModule;
using ECommerce.Presentation.Controllers;
using ECommerce.Presistence.Data.DataSeed;
using ECommerce.Presistence.Data.DbContexts;
using ECommerce.Presistence.IdentityData.DataSeed;
using ECommerce.Presistence.IdentityData.DbContexts;
using ECommerce.Presistence.Repositories;
using ECommerce.Services;
using ECommerce.Services.Abstraction;
using ECommerce.Services.MappingProfiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
#region RegistrationServices 

// Add services to the container.

builder.Services.AddControllers()
    .AddApplicationPart(typeof(ProductsController).Assembly);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ECommerce.API", Version = "v1" });

    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
        }
    );

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                new string[] { }
            },
        }
    );
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy",
        builder =>
        {
            builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
        });
});

builder.Services.AddDbContext<StoreDbConetxt>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Befor Configuring Pipeline  You must Call Seed
// You need to Call Data initializer
builder.Services.AddKeyedScoped<IDataInitializer, DataInitializer>("Default");
builder.Services.AddKeyedScoped<IDataInitializer, IdentityDataInitializer>("Identity");

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
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = ApiResponseFactory.GenerateApiValidationResponse;

});
builder.Services.AddDbContext<StoreIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
});

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<StoreIdentityDbContext>();
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JWTOptions:Issuer"],
            ValidAudience = builder.Configuration["JWTOptions:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWTOptions:SecretKey"]!)
            ),
        };
    });
//builder.Services.AddScoped<IOrderService, OrderService>();
#endregion
var app = builder.Build();


await app.MigrateDataBaseAsync();
await app.MigrateIdentityDataBaseAsync();

await app.SeedDataAsync();
await app.SeedIdentityDataAsync();

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
app.UseCors("DevelopmentPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
#endregion
await app.RunAsync();
