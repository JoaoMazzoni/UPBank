using AddressAPI.Services;
using AddressAPI.Utilis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Configuration;

// Dentro do método ConfigureServices em Startup.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ProjMongoDBAPIDataBaseSettings>(
               builder.Configuration.GetSection(nameof(ProjMongoDBAPIDataBaseSettings)));

builder.Services.AddSingleton<IProjMongoDBAPIDataBaseSettings>(sp =>
    sp.GetRequiredService<IOptions<ProjMongoDBAPIDataBaseSettings>>().Value);

builder.Services.AddSingleton<AddressService>();

// Add services to the container.

builder.Services.AddScoped<AddressService>();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
