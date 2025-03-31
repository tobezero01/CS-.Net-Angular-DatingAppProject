using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
	public class TokenService(IConfiguration configuration) : ITokenService
	{
		public string CreateToken(AppUser appUser)
		{
			var tokenKey = configuration["TokenSettings:TokenKey"] ?? throw new Exception("Cannot access tokenKey from appsettings");
			if (tokenKey.Length < 64) throw new Exception("Your tokenKey needs to be at least 64 characters ");

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
				new(ClaimTypes.Name, appUser.UserName),
			};

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = creds
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}
	}
}