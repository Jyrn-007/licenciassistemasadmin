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

// 🔑 API Key directamente (o desde variable de entorno)
var apiKey = Environment.GetEnvironmentVariable("API_KEY")
             ?? "api_token_2b7efc65-ac43-4401-8395-bf69446cdd5c";

if (string.IsNullOrEmpty(apiKey))
{
    logger.LogError("La API Key no está definida. Configura la variable de entorno API_KEY en Clever Cloud.");
    throw new Exception("La API Key no está definida.");
}

// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Routing
app.UseRouting();

// 🔐 Middleware para proteger la API con API Key
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api"))
    {
        if (!context.Request.Headers.TryGetValue("x-api-key", out var extractedApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("API Key requerida");
            return;
        }

        if (!apiKey.Equals(extractedApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("API Key inválida");
            return;
        }
    }

    await next();
});

// Endpoints
app.UseAuthorization();
app.MapControllers();

app.Run();
