using Microsoft.EntityFrameworkCore;
using ProjectBankAPI.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Log en la consola
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // Guardar logs en archivos diarios
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(builder.Configuration) // Permite leer configuración desde appsettings.json
    .CreateLogger();

// Reemplazar el sistema de logs de .NET con Serilog
builder.Host.UseSerilog();

// Configurar conexión a SQL Server
builder.Services.AddDbContext<BankingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware de Serilog para registrar todas las solicitudes HTTP
app.UseSerilogRequestLogging();

// Configurar Swagger solo en modo desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Iniciar la API
app.Run();
