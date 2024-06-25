using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EmployeeAPI.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EmployeeAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EmployeeAPIContext") ?? throw new InvalidOperationException("Connection string 'EmployeeAPIContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
