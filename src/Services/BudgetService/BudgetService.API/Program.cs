using System.Text;
using FluentValidation;
using BudgetService.Application.Behaviors;
using BudgetService.Application.Commands;
using BudgetService.Application.Interfaces;
using BudgetService.API.Middleware;
using BudgetService.Infrastructure.Persistence;
using BudgetService.Infrastructure.Repositories;
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
    cfg.RegisterServicesFromAssembly(typeof(CreateManualExpenseCommand).Assembly));

// --- FluentValidation (Validator'lar otomatik bulunur) ---
builder.Services.AddValidatorsFromAssembly(typeof(CreateManualExpenseCommandValidator).Assembly);

// Her Command'den önce otomatik validation çalýþtýran pipeline davranýþý
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// --- Repository'ler ---
builder.Services.AddScoped<IBudgetActivityRepository, BudgetActivityRepository>();

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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
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