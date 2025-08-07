using dotenv.net;
using Ecommerce.Application.Common.Mappings;
using Ecommerce.Application.DTOS.User;
using Ecommerce.Application.Repositories.Interfaces;
using Ecommerce.Application.Repositories.Persistence;
using Ecommerce.Application.Services.Contracts.Infrastructure;
using Ecommerce.Application.Services.Impemention;
using Ecommerce.Application.Services.Interfaces;
using Ecommerce.Application.Settings;
using Ecommerce.Infrastructure;
using Ecommerce.Infrastructure.Contracts.Infrastructure;
using Ecommerce.Infrastructure.Contracts.Persistence;
using Ecommerce.Infrastructure.Identity;
using Ecommerce.Infrastructure.Repositories;
using Ecommerce.Infrastructure.Services;
using Ecommerce.Infrastructure.Settings;
using ECommerce.API.BackgroundServices;
using ECommerce.API.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using VNPAY.NET;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

// Setting
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection(CloudinarySettings.SectionName)
);
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName)
);
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection(EmailSettings.SectionName)
);
builder.Services.Configure<GoogleSettings>(
    builder.Configuration.GetSection(GoogleSettings.SectionName)
);
builder.Services.Configure<FrontendSettings>(
    builder.Configuration.GetSection(FrontendSettings.SectionName)
);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(AutoMapperProfie));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ECommerceConnectionstring"));
});




builder.Services.AddSignalR();

//unit of work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// repositories 
builder.Services.AddSingleton<IVnpay, Vnpay>();
builder.Services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IIdentityRepository, IdentityRepository>(); //TokenRepository cũ
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IShippingRepository, ShippingRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IProductSizeRepository, ProductSizeRepository>();
builder.Services.AddScoped<IProductColorRepository, ProductColorRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IBannerRepository, BannerRepository>();


// services 
builder.Services.AddScoped<LocalFileStorageService>(provider =>
{
    var webHostEnvironment = provider.GetRequiredService<IWebHostEnvironment>();
    var webRootPath = webHostEnvironment.WebRootPath;
    return new LocalFileStorageService(webRootPath);
});
builder.Services.AddScoped<CloudinaryImageStorageService>();
builder.Services.AddScoped<IStorageServiceFactory, StorageServiceFactory>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDiscountServices, DiscountServices>();
builder.Services.AddScoped<ICartItemServices, CartItemServices>();
builder.Services.AddScoped<IProductColorServices, ProductColorServices>();
builder.Services.AddScoped<IProductReviewServices, ProductReviewServices>();
builder.Services.AddScoped<IProductSizeServices, ProductSizeServices>();
builder.Services.AddScoped<IProductServices, ProductServices>();
builder.Services.AddScoped<IProductImageServices, ProductImageServices>();
builder.Services.AddScoped<IChatCleanupOrchestratorService, ChatCleanupOrchestratorService>();
builder.Services.AddScoped<IClosedConversationCleanupService, ClosedConversationCleanupService>();
builder.Services.AddScoped<IStalePendingConversationCleanupService, StalePendingConversationCleanupService>();
builder.Services.AddScoped<IPaymentServices, PaymentServices>();
builder.Services.AddScoped<IShippingServices, ShippingServices>();
builder.Services.AddScoped<IEmailServices, EmailServices>();
builder.Services.AddScoped<IExternalAuthService, ExternalAuthService>();
builder.Services.AddScoped<IBannerServices, BannerServices>();



builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") 
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
        });
});

builder.Services.AddIdentityCore<ExtendedIdentityUser>()
    .AddRoles<IdentityRole>()
    .AddTokenProvider<DataProtectorTokenProvider<ExtendedIdentityUser>>("PhucDai")
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(2);
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "GoogleAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            AuthenticationType = "Jwt",
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ClockSkew = TimeSpan.FromSeconds(10) // Disable the default 5 minute clock skew
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/chathub", StringComparison.OrdinalIgnoreCase)))
                {

                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    })
    .AddGoogle("Google",options =>
    {
        options.ClientId = builder.Configuration["Google:ClientId"];
        options.ClientSecret = builder.Configuration["Google:ClientSecret"];
        options.CallbackPath = "/signin-google";

        options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                var externalAuthService = context.HttpContext.RequestServices
                                         .GetRequiredService<IExternalAuthService>();

                var email = context.Principal?.FindFirstValue(ClaimTypes.Email);
                var givenName = context.Principal?.FindFirstValue(ClaimTypes.GivenName);
                var surname = context.Principal?.FindFirstValue(ClaimTypes.Surname);
                var providerKey = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(email))
                {
                    context.Fail("Email claim is missing from Google account.");
                    return;
                }

                // 3. TẠO COMMAND VÀ GỌI APPLICATION SERVICE
                var command = new ExternalAuthCommand
                {
                    Email = email,
                    FirstName = givenName ?? "",
                    LastName = surname ?? "",
                    Provider = "Google",
                    ProviderKey = providerKey
                };

                var authResult = await externalAuthService.AuthenticateAsync(command);
                // 4. XỬ LÝ KẾT QUẢ
                if (!authResult.IsSuccess)
                {
                    context.Fail($"Authentication failed: {authResult.ErrorMessage}");
                    return;
                }

                // 5. GẮN TOKEN VÀO PROPERTIES ĐỂ CHUYỂN TIẾP
                // Một trang callback ở frontend sẽ nhận các token này
                context.Properties.Items["access_token"] = authResult.AccessToken;
                context.Properties.Items["refresh_token"] = authResult.RefreshToken;
                context.Properties.Items["roles"] = string.Join(",", authResult.Roles ?? new List<string>());


                context.Properties.RedirectUri = "/api/Auth/google-callback-handler";
                // Ví dụ: context.Response.Redirect($"{frontendUrl}?token={authResult.AccessToken}");
            },
            OnRemoteFailure = context =>
            {
                // Xử lý khi Google trả về lỗi (ví dụ: user từ chối quyền)
                context.HandleResponse();
                var frontendUrl = builder.Configuration["FrontendUrl"] ?? "http://localhost:5173";
                context.Response.Redirect($"{frontendUrl}/login-failed?error={Uri.EscapeDataString(context.Failure.Message)}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddHostedService<ChatCleanupService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ChatHub>("/chathub");

app.MapControllers();

app.Run();
