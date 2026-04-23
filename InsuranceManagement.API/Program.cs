using InsuranceManagement.Application.Interfaces;
using InsuranceManagement.Application.Services;
using InsuranceManagement.Domain.Entities;
using InsuranceManagement.Infrastructure.Data;
using InsuranceManagement.Infrastructure.Seed;
using InsuranceManagement.Infrastructure.Services;
using InsuranceManagement.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// PORT CONFIG (RENDER FIX)
// ==========================
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    serverOptions.ListenAnyIP(int.Parse(port));
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// ==========================
// SERVICES
// ==========================
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOperatorService, OperatorService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IFinanceService, FinanceService>();
builder.Services.AddScoped<IManagerService, ManagerService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// File storage — Cloudinary (persists across deploys)
builder.Services.AddScoped<IFileStorageService, CloudinaryStorageService>();


builder.Services.AddScoped<PasswordHasherService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// ==========================
// CONTROLLERS
// ==========================
builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new System.Text.Json.Serialization.JsonStringEnumConverter());
});

// ==========================
// DATABASE (POSTGRES)
// ==========================
builder.Services.AddDbContext<InsuranceDbContext>(options =>
{
    var connectionString =
        Environment.GetEnvironmentVariable("DATABASE_URL")
        ?? builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseNpgsql(connectionString);
});

// ==========================
// JWT AUTH
// ==========================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    var key = Environment.GetEnvironmentVariable("JWT_KEY")
              ?? builder.Configuration["Jwt:Key"];

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
    };
});

builder.Services.AddAuthorization();

// ==========================
// SWAGGER (ENABLE FOR DEMO)
// ==========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Insurance API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ==========================
// CORS
// ==========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ==========================
// DB SEEDING
// ==========================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InsuranceDbContext>();

    // Run all pending migrations (correct for production — EnsureCreated skips migrations)
    await dbContext.Database.MigrateAsync();

    await AdminSeeder.SeedAdminAsync(
        dbContext,
        "admin",
        "admin@gmail.com",
        "Admin123!"
    );
}

// ==========================
// MIDDLEWARE
// ==========================
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", (HttpContext context) => Results.Redirect("/swagger"));

app.Run();