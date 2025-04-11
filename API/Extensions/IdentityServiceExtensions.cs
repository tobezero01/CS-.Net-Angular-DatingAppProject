using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
	public static class IdentityServiceExtensions
	{
		public static IServiceCollection AddIdentityServices(this IServiceCollection service,
			IConfiguration configuration)
		{
			service.AddIdentityCore<AppUser>(opt =>
			{
				opt.Password.RequireNonAlphanumeric = false;
			})
				.AddRoles<AppRole>()
				.AddRoleManager<RoleManager<AppRole>>()
				.AddEntityFrameworkStores<DataContext>();

			service.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				var tokenKey = configuration["TokenSettings:TokenKey"] ?? throw new Exception("TokenKey not found");
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
					ValidateIssuer = false,
					ValidateAudience = false
				};
			});

			service.AddAuthorizationBuilder()
				.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
				.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));

			return service;
		}
	}
}