using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AgencyAPI.Data;
using AgencyAPI.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AgencyAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AgencyAPIContext") ?? throw new InvalidOperationException("Connection string 'AgencyAPIContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();


builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IOperationService, OperationService>();


var app = builder.Build();


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
