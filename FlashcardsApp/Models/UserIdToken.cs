using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Models
{
    public class UserIdToken
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }

        public static void ParseTokenToInt(string? userIdString, out int userId)
        {
            if (userIdString == null)
                throw new ArgumentNullException("No userId cookie found");

            bool isIdInt = int.TryParse(userIdString, out userId);
            if (!isIdInt)
                throw new FormatException("userId couldn't be parsed to int");

            if (userId < 0)
                throw new Exception("UserId can't be negative");

        }
    }
}
