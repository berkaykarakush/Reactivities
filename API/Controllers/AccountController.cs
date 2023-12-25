using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using API.DTOs;
using API.Services;
using Domain;
using Infrastructure.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
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
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;
        private readonly EMailSender _emailSender;
        public AccountController(UserManager<AppUser> userManager, TokenService tokenService, IConfiguration config, SignInManager<AppUser> signInManager, EMailSender eMailSender)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _config = config;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://graph.facebook.com")
            };
            _signInManager = signInManager;
            _emailSender = eMailSender;
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
            if (user == null) return Unauthorized("Invalid email");
            if (!user.EmailConfirmed) return Unauthorized("Email not confirmed");
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (result.Succeeded)
            {
                await SetRefreshToken(user);
                return CreateUserObject(user);
            }
            // return unauthorized operation if password is incorrect
            return Unauthorized("Invalid password");
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

            if (!result.Succeeded) return BadRequest("Problem registering user");

            // Retrieve the origin (originating domain) from the request headers
            var origin = Request.Headers["origin"];
            // Generate an email confirmation token for the user
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // Encode the token to be URL-safe using Base64Url encoding
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            // Construct the email verification URL with the token and user's email
            var verifyUrl = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";
            // Construct the email message with the verification link
            var message = $"<p>Please click the below link to verify your email address: </p><p><a href='{verifyUrl}'>Click to verify email</a></p>";
            // Send the email with the verification link to the user's email address
            await _emailSender.SendEmailAsync(user.Email, "Please verify email", message);
            // Return a success message indicating that the email verification link has been resent
            return Ok("Registration Success - Please verify email");
        }

        [AllowAnonymous]
        [HttpPost("verifyEmail")]
        public async Task<IActionResult> VerifyEmail(string token, string email)
        {
            // Find the user by email address
            var user = await _userManager.FindByEmailAsync(email);
            // If the user is not found, return Unauthorized status
            if (user == null) return Unauthorized();
            // Decode the provided token using Base64Url decoding
            var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
            // Confirm the user's email address using the decoded token
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            // If the email confirmation is unsuccessful, return BadRequest status with an error message
            if (!result.Succeeded) return BadRequest("Could not verify email address");
            // Return a success message indicating that the email has been confirmed
            return Ok("Email confirmed - you can now login");
        }

        // Resends the email confirmation link to the specified email address
        [AllowAnonymous]
        [HttpGet("resendEmailConfirmationLink")]
        public async Task<IActionResult> ResendEmailConfirmationLink(string email)
        {
            // Find the user by email address
            var user = await _userManager.FindByEmailAsync(email);
            // If the user is not found, return Unauthorized status
            if (user == null) return Unauthorized();
            // Retrieve the origin (originating domain) from the request headers
            var origin = Request.Headers["origin"];
            // Generate an email confirmation token for the user
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // Encode the token to be URL-safe using Base64Url encoding
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            // Construct the email verification URL with the token and user's email
            var verifyUrl = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";
            // Construct the email message with the verification link
            var message = $"<p>Please click the below link to verify your email address: </p><p><a href='{verifyUrl}'>Click to verify email</a></p>";
            // Send the email with the verification link to the user's email address
            await _emailSender.SendEmailAsync(user.Email, "Please verify email", message);
            // Return a success message indicating that the email verification link has been resent
            return Ok("Email verification link resent");
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            // Get the current user based on the email claim
            var user = await _userManager.Users
                .Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.Email == User.FindFirstValue(ClaimTypes.Email));
            await SetRefreshToken(user);
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
            await SetRefreshToken(user);
            // Return the user object after successful creation
            return CreateUserObject(user);
        }
        // Refreshes the access token using a valid refresh token
        [Authorize]
        [HttpPost("refreshToken")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            // Retrieve the refresh token from the request cookies
            var refreshToken = Request.Cookies["refreshToken"];
            // Retrieve the user, including their refresh tokens, based on the username in the claims
            var user = await _userManager.Users
                .Include(r => r.RefreshTokens)
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.UserName == User.FindFirstValue(ClaimTypes.Name));
            // If the user is not found, return Unauthorized status
            if (user == null) return Unauthorized();
            // Find the old refresh token associated with the provided token
            var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);
            // If the old token is not found or is inactive, return Unauthorized status
            if (oldToken != null && !oldToken.IsActive) return Unauthorized();
            // If the old token is found, mark it as revoked with the current UTC time
            if (oldToken != null) oldToken.Revoked = DateTime.UtcNow;
            // Return the user object after successfully refreshing the token
            return CreateUserObject(user);
        }

        private async Task SetRefreshToken(AppUser user)
        {
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
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