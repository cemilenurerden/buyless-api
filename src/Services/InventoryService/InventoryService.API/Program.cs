using System.Text;
using FluentValidation;
using InventoryService.Application.Behaviors;
using InventoryService.Application.Commands;
using InventoryService.Application.Interfaces;
using InventoryService.API.Middleware;
using InventoryService.Infrastructure.Persistence;
using InventoryService.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// --- Veritabaný ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- MediatR (Commands/Handlers otomatik bulunur) ---
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(AddItemCommand).Assembly));

// --- FluentValidation (Validator'lar otomatik bulunur) ---
builder.Services.AddValidatorsFromAssembly(typeof(AddItemCommandValidator).Assembly);

// Her Command'den önce otomatik validation çalýþtýran pipeline davranýþý
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// --- Repository'ler ---
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IItemWearLogRepository, ItemWearLogRepository>();
builder.Services.AddScoped<IBarcodeProductCacheRepository, BarcodeProductCacheRepository>();

// --- JWT Authentication ---
// AuthService token'ý imzalarken hangi Key/Issuer/Audience kullandýysa,
// burada AYNI deðerleri vermemiz gerekiyor. Aksi halde token "geçersiz" sayýlýr.
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Program.cs içindeki AddSwaggerGen() satýrýný bununla deðiþtir:
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global hata yakalama - en baþta olmalý, sýradaki tüm middleware'lerde
// oluþacak hatalarý (ValidationException dahil) yakalar.
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

// Sýra önemli: Authentication önce, Authorization sonra gelmeli.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();