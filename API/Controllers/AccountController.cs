
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService,
		IMapper _mapper) : BaseApiController
	{
		[HttpPost("register")] // account/register
		public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
		{
			if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");
			var user = _mapper.Map<AppUser>(registerDto);

			user.UserName = registerDto.Username.ToLower();

			var result = await userManager.CreateAsync(user, registerDto.Password);

			if (!result.Succeeded) return BadRequest(result.Errors);

			return new UserDto
			{
				Username = user.UserName,
				Token = await tokenService.CreateToken(user),
				KnowAs = user.KnownAs,
				Gender = user.Gender
			};
		}

		[HttpPost("login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
		{
			var user = await userManager.Users
				.Include(x => x.Photos)
				.FirstOrDefaultAsync(user =>
				user.NormalizedUserName == loginDto.Username.ToUpper());

			if (user == null || user.UserName == null) return Unauthorized("Invalid username");


			return new UserDto
			{
				Username = user.UserName,
				Token = await tokenService.CreateToken(user),
				Gender = user.Gender,
				PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
				KnowAs = user.KnownAs
			};
		}

		private async Task<bool> UserExists(string username)
		{
			return await userManager.Users.AnyAsync(user => user.NormalizedUserName == username.ToUpper());
		}
	}
}