using FlashcardsApp.Dtos;
using FlashcardsApp.Models;

namespace FlashcardsApp.Interfaces
{

    /// <summary>
    /// Model servicing all queries regarding security and authentication.
    /// </summary>
    public interface IAuthModel
    {
        /// <summary>
        /// Function <c>RegisterUser</c> allows to, given validated user credentials, record new user.
        /// </summary>
        /// <param name="userCreds">object containing username and password.</param>
        /// <returns>Returns true, if registering succeeded.</returns>
        public Task<bool> RegisterUser(UserDto userCreds);
        /// <summary>
        /// Function <c>LoginUser</c> allows to, given validated user credentials, create tokens required for 
        /// safe communication. These tokens are then returned to user in http-only cookie or in body. \
        /// In database we update user record with new refreshToken noting that user is logged-in.
        /// </summary>
        /// <param name="userCreds">Validated object containing username and password.</param>
        /// <returns>Returns all tokens required for safe communciation with server.</returns>
        public Task<(string accessToken, RefreshToken refreshToken, UserIdToken userIdToken)> LoginUser(UserDto userCreds);
        /// <summary>
        /// Function <c>LogoutUser</c> allows to, given validated user credentials in httponly cookie, 
        /// to log out user, which means clearing refreshToken in user record in db.
        /// </summary>
        /// <param name="userId">User we want to logout.</param>
        /// <returns>Returns 1 if user was logged out.</returns>
        public Task<int> LogoutUser(int userId);
        /// <summary>
        /// Function <c>RefreshUserTokens</c> refreshes user's tokens. \
        /// Happens everysingle database call to authenticate user and ensure safety. \
        /// Returns new accessToken, refreshToken and same userIdToken but with stamped expiry date.
        /// </summary>
        /// <param name="currRefreshToken">Currently used refresh token by user, to find what user we're referring to.</param>
        /// <returns>Returns new set of tokens.</returns>
        public Task<(string accessToken, RefreshToken refreshToken, UserIdToken userIdToken)> RefreshUserTokens(string currRefreshToken);
    }
}
