using AccountAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AccountsApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AccountsApiContext") ??
                         throw new InvalidOperationException("Connection string 'AccountsApiContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<AccountService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();