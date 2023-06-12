using Azure.Core;
using FlashcardsApp.Dtos;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FlashcardsApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class AuthController : ControllerBase
    {
        private readonly FlashcardsContext _context;
        private readonly IConfiguration _configuration;
        // Get db context and project config
        public AuthController(FlashcardsContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Register with given username and password
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var userList = _context.Users
                .Where(o => o.Username == request.Username).ToList();
            if(userList.Any())
                return BadRequest("Such username already exists");

            CreateHashPassword(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            User user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(user);
        }

        /*
         * Login with given username and password, set http-only refresh token, 
         * reponse with generated access token
         */
        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(UserDto request)
        {
            var userList = _context.Users
                .Where(o => o.Username == request.Username).ToList();
            if (userList.Count() != 1)
                return BadRequest("User not found");

            User user = userList[0];
            bool isVerified = VerifyHashPassword(request.Password, user.PasswordHash, user.PasswordSalt);

            if (isVerified)
            {
                var token = CreateAccessToken(user);
                var refreshToken = CreateRefreshToken();
                SetRefreshToken(refreshToken);

                User refreshedUser = user;
                refreshedUser.RefreshToken = refreshToken.Token;
                refreshedUser.TokenExpires = DateTime.Now.AddMinutes(5);
                _context.Entry(user).CurrentValues.SetValues(refreshedUser);

                try
                {
                    await _context.SaveChangesAsync();
                    return Ok( new { AccessToken = token });
                } catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
                return BadRequest("Invalid password");

        }

        // Refresh refresh token
        // TODO: add client id&secret in order to filter db while checking refresh token
        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var currRefreshToken = Request.Cookies["refreshToken"];

            var userList = _context.Users
                .Where(o => o.RefreshToken == currRefreshToken).ToList();

            if (userList.Any())
            {
                User user = userList[0];

                if (user.TokenExpires < DateTime.Now)
                    return Unauthorized("Token expired.");

                string accessToken = CreateAccessToken(user);
                var newRefreshToken = CreateRefreshToken();
                SetRefreshToken(newRefreshToken);

                User refreshedUser = user;
                refreshedUser.RefreshToken = newRefreshToken.Token;
                refreshedUser.TokenExpires = DateTime.Now.AddMinutes(5);
                _context.Entry(user).CurrentValues.SetValues(refreshedUser);

                try
                {
                    await _context.SaveChangesAsync();
                    return Ok(accessToken);

                } catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
                return Unauthorized("Invalid refresh token.");
        }

        // Create access token
        private string CreateAccessToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(
                    _configuration.GetSection("Authentication:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        // Create refresh token
        private RefreshToken CreateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddMinutes(10)
            };
            return refreshToken;
        }

        // Set http-only refresh token
        private void SetRefreshToken(RefreshToken refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }

        // Hashes password with generated salt
        private void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt) 
        { 
            using (var hmac = new HMACSHA256())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    
        // Validates given password by hashing it with generated previously salt
        private bool VerifyHashPassword(string password, byte[] passwordHash, byte[] passwordSalt) 
        { 
            using (var hmac = new HMACSHA256(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
