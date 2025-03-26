using dimax_front.Application.Interfaces;
using dimax_front.Application.Service;
using dimax_front.Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//CUando se trabaja con referencias circulares
builder.Services.AddControllers().AddJsonOptions(opciones => opciones.JsonSerializerOptions
.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dimax API", Version = "v1" });

    // Configurar el esquema de autenticación JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", 
        Type = SecuritySchemeType.ApiKey, 
        Scheme = "bearer", 
        BearerFormat = "JWT", 
        In = ParameterLocation.Header, 
        Description = "Please enter JWT with Bearer into field"
    });

    // Configurar los requisitos de seguridad para las operaciones
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});


//InjectionDependency
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IInstallationRecords, InstallationRecords>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IAuthService, AuthService>();

//ConnectionDb
builder.Services.AddDbContext<WorkshopDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


//JWT Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["AppSettings:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!))
        };
    });

//AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

var allowOriginsCors = "allowOriginsCors";

builder.Services.AddCors(opt =>
    opt.AddPolicy(name: allowOriginsCors,
        policy =>
        {
            policy.AllowAnyHeader().
            AllowAnyOrigin().
            AllowAnyMethod();
        })
);

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB (por ejemplo)
});

var app = builder.Build();

//Para carpetas dentro del proyecto se puede usar el siguiente código ISS
//var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
//Console.WriteLine($"Uploads Path: {uploadsPath}");

//// Verifica si la carpeta existe antes de usarla
//if (!Directory.Exists(uploadsPath))
//{
//    Console.WriteLine("Carpeta no existe, creando...");
//    Directory.CreateDirectory(uploadsPath);
//}

//// Ahora sí podemos usar PhysicalFileProvider sin problemas
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(uploadsPath),
//    RequestPath = "/uploads"
//})
//;
var uploadsPath = @"C:\ImagenesUploads"; // Ruta absoluta donde están las imágenes

//Para carpetas fuera del proyecto se puede usar el siguiente código ISS
// Verifica si la carpeta existe y crea una si no existe
if (!Directory.Exists(uploadsPath))
{
    Console.WriteLine("Carpeta no existe, creando...");
    Directory.CreateDirectory(uploadsPath);
}

// Configura para servir los archivos estáticos desde la carpeta especificada
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath), // Proveedor de archivos
    RequestPath = "/ImagenesUploads" // El prefijo de la URL para acceder a los archivos
});



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(allowOriginsCors);

//app.UseMiddleware<TokenValidationMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
