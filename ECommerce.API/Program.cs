using ECommerce.API.Data;
using ECommerce.API.Mapping;
using ECommerce.API.Repositories.Impemention;
using ECommerce.API.Repositories.Interface;
using ECommerce.API.Services.Impemention;
using ECommerce.API.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

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

// repositories
builder.Services.AddScoped<ICategoryRepository,CategoryRepository>();
builder.Services.AddScoped<IProductRepository,ProductRepository>();
builder.Services.AddScoped<IProductColorRepository,ProductColorRepository>();
builder.Services.AddScoped<IProductSizeRepository, ProductSizeRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();

// services
builder.Services.AddScoped<IProductServices,ProductServices>();
builder.Services.AddScoped<IProductColorServices,ProductColorServices>();
builder.Services.AddScoped<IProductImageServices,ProductImageServices>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
        });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath,"Uploads")),
    RequestPath = "/Resources"
});

app.UseAuthorization();

app.MapControllers();

app.Run();
