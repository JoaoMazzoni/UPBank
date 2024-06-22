using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AgencyAPI.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AgencyAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AgencyAPIContext") ?? throw new InvalidOperationException("Connection string 'AgencyAPIContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
