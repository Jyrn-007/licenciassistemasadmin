using LicenciaSistemas.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<MySqlContext>();

var app = builder.Build();

// Logger
var logger = app.Services.GetRequiredService<ILogger<Program>>();

// 🔑 Leer API Key desde variable de entorno
var apiKey = builder.Configuration["ApiKey"] ?? Environment.GetEnvironmentVariable("API_KEY");

if (string.IsNullOrEmpty(apiKey))
{
    logger.LogError("La API Key no está definida. Configura la variable de entorno API_KEY en Clever Cloud.");
    throw new Exception("La API Key no está definida. Configura la variable de entorno API_KEY.");
}

// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Routing
app.UseRouting();

// 🔐 Middleware para proteger la API con API Key y log
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        if (!context.Request.Headers.TryGetValue("x-api-key", out var extractedApiKey))
        {
            logger.LogWarning("Acceso denegado: API Key no proporcionada. Path: {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("API Key requerida");
            return;
        }

        // Log para debugging: qué Key llega y cuál se espera
        logger.LogInformation("API Key recibida: {KeyRecibida}, API Key esperada: {KeyEsperada}", extractedApiKey, apiKey);

        if (!apiKey.Equals(extractedApiKey))
        {
            logger.LogWarning("Acceso denegado: API Key inválida. Path: {Path}", context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("API Key inválida");
            return;
        }

        logger.LogInformation("Acceso autorizado a {Path}", context.Request.Path);
    }

    await next();
});

// Autorización y endpoints
app.UseAuthorization();
app.MapControllers();

app.Run();
