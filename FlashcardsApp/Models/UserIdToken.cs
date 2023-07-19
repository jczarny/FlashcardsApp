using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Models
{
    /// <summary>
    /// Object made for easier UserId Token management.
    /// </summary>
    public class UserIdToken
    {
        /// <summary>
        /// Token as user's id.
        /// </summary>
        public string Token { get; set; } = string.Empty;
        /// <summary>
        /// Token's expiriation date.
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Standardised way of parsing userId string to int.
        /// </summary>
        /// <param name="userIdString">userId in string (presumably from cookie).</param>
        /// <param name="userId">correct userId in integer.</param>
        /// <exception cref="ArgumentNullException">Throws when userIdString is empty.</exception>
        /// <exception cref="FormatException">Throws when userId is corrupted and couldn't be parsed.</exception>
        /// <exception cref="Exception">Throws when userId is negative, which should be impossible.</exception>
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
