using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	public class AccountController(DataContext context, ITokenService tokenService,
		IMapper _mapper) : BaseApiController
	{
		[HttpPost("register")] // account/register
		public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
		{
			if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");
			using var hmac = new HMACSHA512();
			var user = _mapper.Map<AppUser>(registerDto);

			user.UserName = registerDto.Username.ToLower();
			user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
			user.PasswordSalt = hmac.Key;

			context.Users.Add(user);
			await context.SaveChangesAsync();

			return new UserDto
			{
				Username = user.UserName,
				Token = tokenService.CreateToken(user),
				KnowAs = user.KnownAs
			};
		}

		[HttpPost("login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
		{
			var user = await context.Users
				.Include(x => x.Photos)
				.FirstOrDefaultAsync(user =>
				user.UserName == loginDto.Username.ToLower());

			if (user == null) return Unauthorized("Invalid username");

			using var hmac = new HMACSHA512(user.PasswordSalt);

			var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

			for (int i = 0; i < computeHash.Length; i++)
			{
				if (computeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
			}

			return new UserDto
			{
				Username = user.UserName,
				Token = tokenService.CreateToken(user),
				PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
				KnowAs = user.KnownAs
			};
		}

		private async Task<bool> UserExists(string username)
		{
			return await context.Users.AnyAsync(user => user.UserName.ToLower() == username.ToLower());
		}
	}
}