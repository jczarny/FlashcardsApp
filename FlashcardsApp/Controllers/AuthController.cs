using FlashcardsApp.Dtos;
using FlashcardsApp.Entities;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class AuthController : ControllerBase
    {
        private readonly AuthModel _auth;

        // Get db context and project config
        public AuthController(FlashcardsContext context, IConfiguration configuration)
        {
            _auth = new AuthModel(context, configuration);
        }

        // Register with given username and password
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] UserDto request)
        {
            // Validate input
            if (request == null)
                return BadRequest("Invalid request");
            int validationResult = _auth.validateUserCredentials(request);
            if (validationResult == 0)
                return BadRequest("Invalid registration input");

            try
            {
                // Register user in database
                bool isSuccess = await _auth.RegisterUser(request);
                if (isSuccess)
                    return Ok();
                else
                    return BadRequest("Unexpected error happened while registering");
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
         * Login with given username and password, set http-only refresh token and userId token, 
         * respond with generated access token.
         */
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto request)
        {
            // Validate request
            int validationResult = _auth.validateUserCredentials(request);
            if (validationResult == 0)
                return BadRequest();

            try
            {
                // Check credentials, prepare tokens
                var tokens = await _auth.LoginUser(request);

                // set http-only tokens and respond with access token
                SetRefreshToken(tokens.refreshToken);
                SetUserIdToken(tokens.userIdToken);
                return Ok(new { AccessToken = tokens.accessToken });
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

        // Logout, clear user's tokens.
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Validate userId
                var userIdString = Request.Cookies["userId"];
                UserIdToken.ParseTokenToInt(userIdString, out int userId);

                // Try logging out, invalidate refresh token in database
                int result = await _auth.LogoutUser(userId);
                // Delete client-side tokens
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

        // Refresh all tokens
        // TODO: add client id&secret in order to filter db while checking refresh token
        [HttpPost("refresh-tokens")]
        public async Task<ActionResult<string>> RefreshTokens()
        {
            try
            {
                // Validate tokens
                var currRefreshToken = Request.Cookies["refreshToken"];
                if (currRefreshToken == null)
                    return Unauthorized("No refresh token given");

                var currUserIdToken = Request.Cookies["userId"];
                UserIdToken.ParseTokenToInt(currUserIdToken, out int userId); // just to validate id

                // Check correctness of tokens and prepare new ones
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

        // Delete client-side cookies
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
