using Azure.Core;
using Azure.Identity;
using FlashcardsApp.Dtos;
using FlashcardsApp.Entities;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace FlashcardsApp.Models
{
    public class Auth
    {
        private readonly FlashcardsContext _context;
        private readonly IConfiguration _configuration;

        public Auth(FlashcardsContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // given credentials create new user
        public async Task<User> RegisterUser(UserDto userCreds)
        {
            bool doesUserExists = CheckIfSuchUserExists(userCreds.Username, _context);
            if (doesUserExists) {
                throw new ArgumentException("Such username already exists");
            }

            CreateHashPassword(userCreds.Password, out byte[] passwordHash, out byte[] passwordSalt);
            User newUser = new User
            {
                Username = userCreds.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            try
            {
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return newUser;
            }
            catch
            {
                throw;
            }
        }

        // validate user credentials to login, return his userId and Access Token
        public async Task<(string accessToken, RefreshToken refreshToken, int id)> LoginUser(UserDto userCreds)
        {
            bool doesUserExists = CheckIfSuchUserExists(userCreds.Username, _context);
            if (!doesUserExists)
            {
                throw new ArgumentException("Such username does not exist");
            }
            User foundUser = _context.Users.Where(o => o.Username == userCreds.Username).ToList()[0];
            
            bool isPwdCorrect = VerifyHashPassword(userCreds.Password, foundUser.PasswordHash, foundUser.PasswordSalt);
            if(isPwdCorrect)
            {
                var token = CreateAccessToken(foundUser);
                var refreshToken = CreateRefreshToken();

                User refreshedUser = foundUser;
                refreshedUser.RefreshToken = refreshToken.Token;
                refreshedUser.TokenExpires = DateTime.Now.AddMinutes(5);
                _context.Entry(foundUser).CurrentValues.SetValues(refreshedUser);

                try
                {
                    await _context.SaveChangesAsync();
                    return (token, refreshToken, foundUser.Id);
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                throw new IOException("Invalid password");
            }
        }

        public async Task<int> LogoutUser(int userId)
        {
            var userList = _context.Users
                .Where(o => o.Id == userId).ToList();
            if (userList.Count != 1)
            {
                throw new ArgumentException("Something went wrong and we couldnt find such user");
            }

            var user = userList[0];
            User refreshedUser = user;
            refreshedUser.RefreshToken = null;
            try
            {
                _context.Entry(user).CurrentValues.SetValues(refreshedUser);
                await _context.SaveChangesAsync();
                return 1;
            } catch
            {
                throw;
            }
        }

        public async Task<(string accessToken, RefreshToken refreshToken)> RefreshUserToken(string currRefreshToken)
        {
            var userList = _context.Users
                .Where(o => o.RefreshToken == currRefreshToken).ToList();

            if (!userList.Any())
                throw new UnauthorizedAccessException("Invalid refresh token.");

            User user = userList[0];

            if (user.TokenExpires < DateTime.Now)
                throw new UnauthorizedAccessException("Token expired.");

            string accessToken = CreateAccessToken(user);
            var newRefreshToken = CreateRefreshToken();

            User refreshedUser = user;
            refreshedUser.RefreshToken = newRefreshToken.Token;
            refreshedUser.TokenExpires = DateTime.Now.AddMinutes(5);
            try
            {
                _context.Entry(user).CurrentValues.SetValues(refreshedUser);
                await _context.SaveChangesAsync();
                return (accessToken, newRefreshToken);
            }
            catch
            {
                throw;
            }
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
                expires: DateTime.Now.AddMinutes(5),
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
                Expires = DateTime.Now.AddMinutes(25)
            };
            return refreshToken;
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

        private bool CheckIfSuchUserExists(string username, FlashcardsContext dbcontext)
        {
            var userList = dbcontext.Users
                .Where(o => o.Username == username).ToList();
            if (userList.Any())
                return true;
            else
                return false;
        }

        public int validateUserCredentials(UserDto user)
        {
            string regexUsername = "^[a-zA-Z0-9]([a-zA-Z0-9]){3,18}[a-zA-Z0-9]$";
            string regexPassword = "^(?=.*?[A-Z])(?=.*?[a-z]).{8,}$";

            Console.WriteLine(user.Username, !Regex.IsMatch(user.Username, regexUsername));
            Console.WriteLine(user.Password, !Regex.IsMatch(user.Password, regexPassword));

            if (user == null) return 0;
            if (!Regex.IsMatch(user.Username, regexUsername)) return 0;
            if (!Regex.IsMatch(user.Password, regexPassword)) return 0;
            return 1;
        }
    }
}
