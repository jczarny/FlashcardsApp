using Azure.Core;
using FlashcardsApp.Dtos;
using FlashcardsApp.Entities;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace FlashcardsApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class AuthController : ControllerBase
    {
        private readonly Auth _auth;

        // Get db context and project config
        public AuthController(FlashcardsContext context, IConfiguration configuration)
        {
            _auth = new Auth(context, configuration);
        }

        // Register with given username and password
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            int validationResult = _auth.validateUserCredentials(request);
            if (validationResult == 0)
                return BadRequest();

            try
            {
                User user = await _auth.RegisterUser(request);
                return Ok(user);
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException){
                    return Conflict(ex.Message);
                }
                else
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        /*
         * Login with given username and password, set http-only refresh token, 
         * response with generated access token
         */
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto request)
        {
            int validationResult = _auth.validateUserCredentials(request);
            if (validationResult == 0)
                return BadRequest();

            try
            {
                var user = await _auth.LoginUser(request);
                SetRefreshToken(user.refreshToken);
                SetUserIdToken(user.userIdToken);
                return Ok();
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException)
                {
                    return Conflict(ex.Message);
                }
                else if (ex is IOException)
                {
                    return Unauthorized("Invalid password");
                }
                else
                {
                    return BadRequest(ex.Message);
                }
            }

        }

        // Logout, clear user's refresh token.
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userIdString = Request.Cookies["userId"];
                UserIdToken.ParseTokenToInt(userIdString, out int userId);

                int result = await _auth.LogoutUser(userId);
                DeleteTokens();
                return Ok();
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException)
                {
                    return Conflict(ex.Message);
                }
                else
                {
                    return BadRequest(ex.Message);
                }
            }

        }

        // Refresh refresh token
        // TODO: add client id&secret in order to filter db while checking refresh token
        [HttpPost("refresh-tokens")]
        public async Task<ActionResult<string>> RefreshTokens()
        {
            try
            {
                var currRefreshToken = Request.Cookies["refreshToken"];
                if (currRefreshToken == null)
                    return Unauthorized("No refresh token given");

                var currUserIdToken = Request.Cookies["userId"];
                UserIdToken.ParseTokenToInt(currUserIdToken, out int userId); // just to validate id

                var newTokens = await _auth.RefreshUserTokens(currRefreshToken);
                SetRefreshToken(newTokens.refreshToken);
                SetUserIdToken(newTokens.userIdToken);

                return Ok(new { AccessToken = newTokens.accessToken });
            }
            catch (Exception ex)
            {
                if(ex is UnauthorizedAccessException)
                {
                    return Unauthorized(ex.Message);
                }
                else
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        // Set http-only cookie storing refresh token
        private void SetRefreshToken(RefreshToken refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }

        // Set http-only cookie storing user id 
        private void SetUserIdToken(UserIdToken userIdToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = userIdToken.Expires
            };
            Response.Cookies.Append("userId", userIdToken.Token, cookieOptions);
        }

        private void DeleteTokens()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(-1)
            };
            Response.Cookies.Append("userId", "null", cookieOptions);
            Response.Cookies.Append("refreshToken", "null", cookieOptions);
        }
    }
}
