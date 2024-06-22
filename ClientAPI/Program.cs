using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CustomerAPI.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<CustomerAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CustomerAPIContext") ?? throw new InvalidOperationException("Connection string 'CustomerAPIContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
