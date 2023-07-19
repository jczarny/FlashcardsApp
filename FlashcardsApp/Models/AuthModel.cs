using FlashcardsApp.Dtos;
using FlashcardsApp.Entities;
using FlashcardsApp.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace FlashcardsApp.Models
{
    /// <summary>
    /// Model servicing all queries regarding security and authentication.
    /// </summary>
    public class AuthModel : IAuthModel
    {
        /// <summary>
        /// Entity framework context.
        /// </summary>
        private readonly FlashcardsContext _context;
        /// <summary>
        /// Configuration for authentication purposes. 
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor injecting database context and configuration for authentication token.
        /// </summary>
        /// <param name="context">Entity Framework context.</param>
        /// <param name="configuration">Project configuration containing authentication token.</param>
        public AuthModel(FlashcardsContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Function <c>RegisterUser</c> allows to, given validated user credentials, record new user.
        /// </summary>
        /// <param name="userCreds">object containing username and password.</param>
        /// <returns>Returns true, if registering succeeded.</returns>
        /// <exception cref="ArgumentException">Raise if username is taken.</exception>
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

        /// <summary>
        /// Function <c>LoginUser</c> allows to, given validated user credentials, create tokens required for 
        /// safe communication. These tokens are then returned to user in http-only cookie or in body. \
        /// In database we update user record with new refreshToken noting that user is logged-in.
        /// </summary>
        /// <param name="userCreds">Validated object containing username and password.</param>
        /// <returns>Returns all tokens required for safe communciation with server.</returns>
        /// <exception cref="ArgumentException">Thrown where such user does not exist.</exception>
        /// <exception cref="IOException">When given password is invalid.</exception>
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

 
        /// <summary>
        /// Function <c>LogoutUser</c> allows to, given validated user credentials in httponly cookie, 
        /// to log out user, which means clearing refreshToken in user record in db.
        /// </summary>
        /// <param name="userId">User we want to logout.</param>
        /// <returns>Returns 1 if user was logged out.</returns>
        /// <exception cref="ArgumentException">Raised when we couldn't find such user in database.</exception>
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

        /// <summary>
        /// Function <c>RefreshUserTokens</c> refreshes user's tokens. \
        /// Happens everysingle database call to authenticate user and ensure safety. \
        /// Returns new accessToken, refreshToken and same userIdToken but with stamped expiry date.
        /// </summary>
        /// <param name="currRefreshToken">Currently used refresh token by user, to find what user we're referring to.</param>
        /// <returns>Returns new set of tokens.</returns>
        /// <exception cref="UnauthorizedAccessException">When refresh token was invalid or already expired.</exception>
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


        /// <summary>
        /// Creates new access token.
        /// </summary>
        /// <param name="user">user to which we're assigning new access token.</param>
        /// <returns>Returns new access token.</returns>
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

        /// <summary>
        /// Creates new refresh token.
        /// </summary>
        /// <returns>Returns object with new refresh token and its expiration date set.</returns>
        private RefreshToken CreateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddMinutes(5)
            };
            return refreshToken;
        }

        /// <summary>
        /// Creates new userId token.
        /// </summary>
        /// <param name="userId">Id of user we want to create token for.</param>
        /// <returns>Returns object with given userId and expiration date set.</returns>
        private UserIdToken CreateUserIdToken(string userId)
        {
            var userIdToken = new UserIdToken
            {
                Token = userId,
                Expires = DateTime.Now.AddMinutes(5)
            };
            return userIdToken;
        }

        /// <summary>
        /// Hashes password with given salt.
        /// </summary>
        /// <param name="password">Not encrypted, plain password.</param>
        /// <param name="passwordHash">Hashed password we'll be storing in database.</param>
        /// <param name="passwordSalt">Salt we used for hashing password also stored in database.</param>
        private void CreateHashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        /// <summary>
        /// Validates given password by hashing it with generated previously salt.
        /// </summary>
        /// <param name="password">Not encrypted, plain password.</param>
        /// <param name="passwordHash">Hashed password we're storing in database.</param>
        /// <param name="passwordSalt">Salt we used to create passwordHash.</param>
        /// <returns>Returns True if password hashed with given salt gives us passwordHash, otherwise false. </returns>
        private bool VerifyHashPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        /// <summary>
        /// Checks if user with given username exists, used in login and register.
        /// </summary>
        /// <param name="username">Username we're checking if exists in database.</param>
        /// <param name="dbcontext">Context of database.</param>
        /// <returns>Returns True if such username exists, otherwise false. </returns>
        private bool CheckIfSuchUserExists(string username, FlashcardsContext dbcontext)
        {
            var userList = dbcontext.Users
                .Where(o => o.Username == username).ToList();
            if (userList.Any())
                return true;
            else
                return false;
        }

        /// <summary>
        /// Validates user credentials. (make sure they're as restrictive as frontend validation)
        /// </summary>
        /// <param name="user">User credentials.</param>
        /// <returns>Returns 1 if validation succeedes, otherwise 0.</returns>
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
