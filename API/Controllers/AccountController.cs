using System.Security.Claims;
using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;
        public AccountController(UserManager<AppUser> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }
        [AllowAnonymous] // override authentication
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // user control
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            // return unauthorized operation if user is not found
            if (user == null) return Unauthorized();
            // perform password verification if the user exists
            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            // if the password is correct, perform the necessary input operations
            if (result)
            {
                return CreateUserObject(user);
            }
            // return unauthorized operation if password is incorrect
            return Unauthorized();
        }

        [AllowAnonymous] // override authentication
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // check username
            if (await _userManager.Users.AnyAsync(u => u.UserName == registerDto.Username))
                return BadRequest("Username is already taken!");

            // create new user            
            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Username
            };
            // add user
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            // if user create is successfull
            if (result.Succeeded)
            {
                return CreateUserObject(user);
            }
            // if user create is not successfull
            return BadRequest(result.Errors);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            return CreateUserObject(user);
        }

        private UserDto CreateUserObject(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Image = null,
                Token = _tokenService.CreateToken(user),
                Username = user.UserName
            };
        }
    }
}