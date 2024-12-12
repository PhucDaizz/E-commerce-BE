using ECommerce.API.Data;
using ECommerce.API.Mapping;
using ECommerce.API.Repositories.Impemention;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Impemention;
using ECommerce.API.Services.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(AutoMapperProfie));

builder.Services.AddDbContext<ECommerceDbContext>(options =>
{                               
    options.UseSqlServer(builder.Configuration.GetConnectionString("ECommerceConnectionstring"));
});

builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();
builder.Services.AddScoped<IProductRepository,ProductRepository>();
builder.Services.AddScoped<IProductColorRepository,ProductColorRepository>();
builder.Services.AddScoped<IProductSizeRepository, ProductSizeRepository>();

builder.Services.AddScoped<IProductServices,ProductServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
