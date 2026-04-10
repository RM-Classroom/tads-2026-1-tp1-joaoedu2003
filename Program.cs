using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Locadora de Veículos API",
        Version = "v1",
        Description = "Sistema de venda de veículos."
    });
});

builder.Services.AddDbContext<LocadoraContext>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Locadora API v1");
    c.RoutePrefix = string.Empty;
});

app.UseAuthorization();
app.MapControllers();

app.Run();
