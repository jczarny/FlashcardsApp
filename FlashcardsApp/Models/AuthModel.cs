using FlashcardsApp.Dtos;
using FlashcardsApp.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace FlashcardsApp.Models
{
    public class AuthModel
    {
        private readonly FlashcardsContext _context;
        private readonly IConfiguration _configuration;

        public AuthModel(FlashcardsContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Given credentials, Create new user
        public async Task<bool> RegisterUser(UserDto userCreds)
        {
            // Check if such user already exists
            bool doesUserExists = CheckIfSuchUserExists(userCreds.Username, _context);
            if (doesUserExists) {
                throw new ArgumentException("Such username already exists");
            }

            // Prepare hashed password with its salt
            CreateHashPassword(userCreds.Password, out byte[] passwordHash, out byte[] passwordSalt);
            
            // Prepare user record
            User newUser = new User
            {
                Username = userCreds.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            try
            {
                // Add username to database
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        /*  Validate user credentials to login
         *  Return all necessary tokens
         */
        public async Task<(string accessToken, RefreshToken refreshToken, UserIdToken userIdToken)> LoginUser(UserDto userCreds)
        {
            // Check if such user even exists
            bool doesUserExists = CheckIfSuchUserExists(userCreds.Username, _context);
            if (!doesUserExists)
            {
                throw new ArgumentException("Such username does not exist");
            }
            User foundUser = _context.Users.Where(o => o.Username == userCreds.Username).ToList()[0];
            
            // Check his password
            bool isPwdCorrect = VerifyHashPassword(userCreds.Password, foundUser.PasswordHash, foundUser.PasswordSalt);
            if(isPwdCorrect)
            {
                // Prepare all tokens
                var accessToken = CreateAccessToken(foundUser);
                var refreshToken = CreateRefreshToken();
                var userIdToken = CreateUserIdToken(foundUser.Id.ToString());

                // Prepare user record with his assigned refresh token
                User refreshedUser = foundUser;
                refreshedUser.RefreshToken = refreshToken.Token;
                refreshedUser.TokenExpires = DateTime.Now.AddMinutes(5);
                _context.Entry(foundUser).CurrentValues.SetValues(refreshedUser);

                try
                {
                    await _context.SaveChangesAsync();
                    return (accessToken, refreshToken, userIdToken);
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

        /*
         * Logouts given user by clearing his refresh token
         * Returns 1 if executed properly
         */
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

        /*
         * Refresh user's token.
         * Happens everysingle database call to authenticate user and ensure safety.
         * Returns new accessToken, refreshToken and same userIdToken but with stamped expiry date.
         */
        public async Task<(string accessToken, RefreshToken refreshToken, UserIdToken userIdToken)> RefreshUserTokens(string currRefreshToken)
        {
            // Authenticate user by checking if refresh token is valid
            var userList = _context.Users
                .Where(o => o.RefreshToken == currRefreshToken).ToList();

            if (!userList.Any())
                throw new UnauthorizedAccessException("Invalid refresh token.");

            User user = userList[0];

            if (user.TokenExpires < DateTime.Now)
                throw new UnauthorizedAccessException("Token expired.");

            // Prepare new tokens
            string accessToken = CreateAccessToken(user);
            var newRefreshToken = CreateRefreshToken();
            var newUserIdToken = CreateUserIdToken(user.Id.ToString());

            // Update user record with new refresh token
            User refreshedUser = user;
            refreshedUser.RefreshToken = newRefreshToken.Token;
            refreshedUser.TokenExpires = DateTime.Now.AddMinutes(5);
            try
            {
                _context.Entry(user).CurrentValues.SetValues(refreshedUser);
                await _context.SaveChangesAsync();
                return (accessToken, newRefreshToken, newUserIdToken);
            }
            catch
            {
                throw;
            }
        }


        // Create new access token
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
                Expires = DateTime.Now.AddMinutes(5)
            };
            return refreshToken;
        }

        // Create userId token
        private UserIdToken CreateUserIdToken(string userId)
        {
            var userIdToken = new UserIdToken
            {
                Token = userId,
                Expires = DateTime.Now.AddMinutes(5)
            };
            return userIdToken;
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

        // Checks if user exists, used in login and register
        private bool CheckIfSuchUserExists(string username, FlashcardsContext dbcontext)
        {
            var userList = dbcontext.Users
                .Where(o => o.Username == username).ToList();
            if (userList.Any())
                return true;
            else
                return false;
        }

        // Validate user credentials (make sure they're identical with frontend validation)
        public int validateUserCredentials(UserDto user)
        {
            string regexUsername = "^[a-zA-Z0-9]([a-zA-Z0-9._]){3,18}[a-zA-Z0-9]$";
            string regexPassword = "^([a-zA-Z0-9@*#]{8,16})$";

            Console.WriteLine(user.Username, !Regex.IsMatch(user.Username, regexUsername));
            Console.WriteLine(user.Password, !Regex.IsMatch(user.Password, regexPassword));

            if (user == null) return 0;
            if (!Regex.IsMatch(user.Username, regexUsername)) return 0;
            if (!Regex.IsMatch(user.Password, regexPassword)) return 0;
            return 1;
        }
    }
}
