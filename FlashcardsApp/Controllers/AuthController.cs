using FlashcardsApp.Dtos;
using FlashcardsApp.Entities;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Controllers
{
    /// <summary>
    /// Class <c>AuthController</c> manages user authentication by handling logging in and out, registering 
    /// and refreshing user's tokens.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Model used for calling database queries.
        /// </summary>
        private readonly AuthModel _auth;

        /// <summary>
        /// Constructor injecting database context and configuration for connectionString.
        /// </summary>
        /// <param name="context">Entity Framework context.</param>
        /// <param name="configuration">Project configuration containing connectionString.</param>
        public AuthController(FlashcardsContext context, IConfiguration configuration)
        {
            _auth = new AuthModel(context, configuration);
        }

        /// <summary>
        /// Function <c>Register</c> validates credentials, and records new user with given username and password.
        /// </summary>
        /// <param name="request">Given by user username and password.</param>
        /// <returns> Returns empty status code 200 when success, \
        /// Conflict(409) when such username is taken, \
        /// in other cases BadRequest(400). </returns>
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

        /// <summary>
        /// Function <c>Login</c> validates credentials and sets http-only refresh token and userId token.
        /// </summary>
        /// <param name="request">Given by user username and password.</param>
        /// <returns> Returns Ok() with new AccessToken in body, and before that, sets 
        /// http-only cookies with userId and refreshToken. \
        /// If no such user found, return Conflict(409). \
        /// If given password is invalid return Unauthorized(401). \
        /// in other cases return BadRequest(400). </returns>
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

        /// <summary>
        /// Function <c>Logout</c> clears user's tokens.
        /// </summary>
        /// <returns> Returns Ok() when success. \
        /// Conflict(409) when no such user found. \
        /// In other cases BadRequest(400) </returns>
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

        /// <summary>
        /// Function <c>RefreshTokens</c> validates http-only refreshToken, and if its valid then refreshes all tokens.
        /// </summary>
        /// <returns> Returns Ok() with new AccessToken in body, and before that, sets 
        /// http-only cookies with userId and refreshToken. \
        /// Unauthorized(401) when no refresh token given in cookie, or thrown by UserModel. \
        /// In other cases BadRequest(400). </returns>
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

        /// <summary>
        /// Function <c>SetRefreshToken</c> sets http-only cookie storing refresh token.
        /// </summary>
        /// <param name="refreshToken">Token to be set as a cookie.</param>
        private void SetRefreshToken(RefreshToken refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }

        /// <summary>
        /// Function <c>SetUserIdToken</c> sets http-only cookie storing user id.
        /// </summary>
        /// <param name="userIdToken">Token to be set as a cookie.</param>
        private void SetUserIdToken(UserIdToken userIdToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = userIdToken.Expires
            };
            Response.Cookies.Append("userId", userIdToken.Token, cookieOptions);
        }

        /// <summary>
        /// Function <c>DeleteTokens</c> deletes authentication tokens client-side.
        /// </summary>
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
