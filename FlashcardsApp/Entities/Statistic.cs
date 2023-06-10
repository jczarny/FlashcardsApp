using FlashcardsApp.Models;

namespace FlashcardsApp.Entities
{
    public class Statistic
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public int TotalGuesses { get; set; }
        public int CorrectGuesses { get; set; }
        public int WrongGuesses { get; set; }

    }
}
