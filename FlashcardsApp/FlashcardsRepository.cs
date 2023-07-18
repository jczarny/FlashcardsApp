using FlashcardsApp.Interfaces;
using FlashcardsApp.Models;

namespace FlashcardsApp
{
    public class FlashcardsRepository : IFlashcardsRepository
    {
        public IDeckModel _deckModel { get; set; }
        public ILearnModel _learnModel { get; set; }
        public IUserModel _userModel { get; set; }

        public FlashcardsRepository(string connectionString) {
            _deckModel = new DeckModel(connectionString);
            _learnModel = new LearnModel(connectionString);
            _userModel = new UserModel(connectionString);
        }

    }
}
