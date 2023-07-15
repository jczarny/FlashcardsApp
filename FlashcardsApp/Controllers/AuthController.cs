﻿using Azure.Core;
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
                return Ok(new { AccessToken = user.accessToken, UserId = user.id });
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
                string userId = Request.Headers["userId"].ToString();
                int result = await _auth.LogoutUser(Int32.Parse(userId));
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
        [HttpPost("refresh-token"), Authorize]
        public async Task<ActionResult<string>> RefreshToken()
        {
            try
            {
                var currRefreshToken = Request.Cookies["refreshToken"];
                if (currRefreshToken == null)
                    return BadRequest("No refresh token given");

                var newTokens = await _auth.RefreshUserToken(currRefreshToken);
                SetRefreshToken(newTokens.refreshToken);
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

    }
}
