using FlashcardsApp.Dtos;
using FlashcardsApp.Models;

namespace FlashcardsApp.Interfaces
{
    public interface IAuthModel
    {
        public Task<bool> RegisterUser(UserDto userCreds);
        public Task<(string accessToken, RefreshToken refreshToken, UserIdToken userIdToken)> LoginUser(UserDto userCreds);
        public Task<int> LogoutUser(int userId);
        public Task<(string accessToken, RefreshToken refreshToken, UserIdToken userIdToken)> RefreshUserTokens(string currRefreshToken);
    }
}
