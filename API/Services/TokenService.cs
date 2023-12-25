using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Domain;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    // JWT Token islemlerinin gerceklestirildigi sinif
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// JWT Token creation method
        /// </summary>
        /// <param name="user">user information</param>
        /// <returns>Return token value</returns>
        public string CreateToken(AppUser user)
        {
            // Create the clauses that will be contained in the token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName), // username request
                new Claim(ClaimTypes.NameIdentifier, user.Id), // id request
                new Claim(ClaimTypes.Email, user.Email) // email request
            };
            // generates the symmetric key to be used for signing the token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenKey"]));
            // determining the algorithm to be used for signing the token and creating the signature information
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            // Create an object to be used for the general configuration of the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // adds clauses into token
                Expires = DateTime.UtcNow.AddMinutes(1), // sets the validity of the token to 7 days
                SigningCredentials = creds // adds the token's signature information
            };
            // token manager to be used to create the token
            var tokenHandler = new JwtSecurityTokenHandler();
            // tokenin olusturulmasi
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // converting token to text format
            return tokenHandler.WriteToken(token);
        }
        // Generates a secure refresh token using a cryptographically strong random number generator
        public RefreshToken GenerateRefreshToken()
        {
            // Create a byte array to store random numbers
            var randomNumber = new byte[32];
            // Using statement ensures that the RandomNumberGenerator is properly disposed after use
            using var rng = RandomNumberGenerator.Create();
            // Fill the byte array with random numbers
            rng.GetBytes(randomNumber);
            // Convert the byte array to a base64-encoded string and return a new RefreshToken
            return new RefreshToken { Token = Convert.ToBase64String(randomNumber) };
        }
    }
}