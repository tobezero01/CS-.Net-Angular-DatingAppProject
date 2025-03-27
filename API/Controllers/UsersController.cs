using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	[Authorize]
	public class UsersController(IUserRepository _userRepository, IMapper _mapper, PhotoService photoService) : BaseApiController
	{
		[HttpGet]
		public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
		{
			var users = await _userRepository.GetMembersAsync();

			return Ok(users);
		}

		[HttpGet("{id:int}")]
		public async Task<ActionResult<MemberDto>> GetUser(int id)
		{
			var user = await _userRepository.GetUserByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			return _mapper.Map<MemberDto>(user);
		}

		[HttpGet("{username}")]
		public async Task<ActionResult<MemberDto>> GetUserByName(string username)
		{
			var user = await _userRepository.GetMemberAsync(username);
			if (user == null)
			{
				return NotFound();
			}

			return user;
		}

		[HttpPut]
		public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
		{
			var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (username == null)
			{
				return BadRequest("No username found in token");
			}
			var user = await _userRepository.GetUserByUsernameAsync(username);
			if (user == null)
			{
				return BadRequest("Could you find user");
			}
			_mapper.Map(memberUpdateDto, user);

			if (await _userRepository.SaveAllAsync()) return NoContent();
			return BadRequest("Failed to update the user");
		}

		[HttpPost("add-photo")]
		public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
		{
			var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
			if (user == null) return BadRequest("Cannot update user");
			var res = await photoService.AddPhotoAsync(file);
			if (res.Error != null) return BadRequest(res.Error.Message);
			var photo = new Photo
			{
				Url = res.SecureUrl.AbsolutePath,
				PublicId = res.PublicId
			};
			user.Photos.Add(photo);

			if (await _userRepository.SaveAllAsync()) return _mapper.Map<PhotoDto>(photo);
			return BadRequest("Problem adding photo");
		}
	}
}