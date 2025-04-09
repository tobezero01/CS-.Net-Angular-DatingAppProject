using System.Text;
using API.Data;
using API.Data.Migrations;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.Middleware;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services extension to the container.
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);


var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod()
	.WithOrigins("http://localhost:4200", "https://localhost:4200"));
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
	var context = services.GetRequiredService<DataContext>();
	var userManage = services.GetRequiredService<UserManager<AppUser>>();
	var roleManage = services.GetRequiredService<RoleManager<AppRole>>();
	await context.Database.MigrateAsync();
	await SeedData.SeedUsers(userManage, roleManage);
}
catch (Exception ex)
{
	var logger = services.GetRequiredService<ILogger<Program>>();
	logger.LogError(ex, "An error occurred during migration");
}

app.Run();