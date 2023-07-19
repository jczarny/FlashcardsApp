using FlashcardsApp.Models;

namespace FlashcardsApp.Entities
{
    /// <summary>
    /// Some statistic for user \
    /// Not yet implemented usage.
    /// </summary>
    public class Statistic
    {
        /// <summary>
        /// Concerned user.
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Used in Entity framework modeling.
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// Today's day.
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// How many guesses in learning process user made today.
        /// </summary>
        public int TotalGuesses { get; set; }
        /// <summary>
        /// How many guesses were correct (not try again) today.
        /// </summary>
        public int CorrectGuesses { get; set; }
        /// <summary>
        /// How many guesses were wrong (try again) today.
        /// </summary>
        public int WrongGuesses { get; set; }

    }
}
