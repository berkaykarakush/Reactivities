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
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;
        public AccountController(UserManager<AppUser> userManager, TokenService tokenService, IConfiguration config)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _config = config;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://graph.facebook.com")
            };
        }

        [AllowAnonymous] // override authentication
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // Check if the user exists
            var user = await _userManager.Users
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);
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

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            // Get the current user based on the email claim
            var user = await _userManager.Users
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.Email == User.FindFirstValue(ClaimTypes.Email));
            // Return user information
            return CreateUserObject(user);
        }

        [AllowAnonymous]
        [HttpPost("fbLogin")]
        public async Task<ActionResult<UserDto>> FacebookLogin(string accessToken)
        {
            // Concatenate Facebook AppId and ApiSecret for verification
            var fbVerifyKeys = _config["Facebook:AppId"] + "|" + _config["Facebook:ApiSecret"];
            // Verify the Facebook access token using Facebook Graph API
            var verifyTokenResponse = await _httpClient.GetAsync($"debug_token?input_token={accessToken}&access_token={fbVerifyKeys}");
            // If the token verification is unsuccessful, return Unauthorized status
            if (!verifyTokenResponse.IsSuccessStatusCode) return Unauthorized();
            // Construct the Facebook Graph API URL to retrieve user information
            var fbUrl = $"me?access_token={accessToken}&fields=name,email,picture.width(100).height(100)";
            // Retrieve user information from Facebook Graph API
            var fbInfo = await _httpClient.GetFromJsonAsync<FacebookDto>(fbUrl);
            // Check if a user with the same email already exists in the application database
            var user = await _userManager.Users.Include(u => u.Photos).FirstOrDefaultAsync(p => p.Email == fbInfo.Email);
            // If the user already exists, return the user object
            if (user != null) return CreateUserObject(user);
            // If the user does not exist, create a new user with Facebook information
            user = new AppUser
            {
                DisplayName = fbInfo.Name,
                Email = fbInfo.Email,
                UserName = fbInfo.Email,
                Photos = new List<Photo>
                {
                    new Photo
                    {
                        Id = $"fb_{fbInfo.Id}",
                        Url = fbInfo.Picture.Data.Url,
                        IsMain = true
                    }
                }
            };
            // Create the user in the application database
            var result = await _userManager.CreateAsync(user);
            // If user creation is unsuccessful, return a BadRequest status with an error message
            if (!result.Succeeded) return BadRequest("Problem creating user account");
            // Return the user object after successful creation
            return CreateUserObject(user);
        }

        // Helper method to create a UserDto object from an AppUser
        private UserDto CreateUserObject(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Image = user?.Photos?.FirstOrDefault(u => u.IsMain)?.Url,
                Token = _tokenService.CreateToken(user),
                Username = user.UserName
            };
        }
    }
}