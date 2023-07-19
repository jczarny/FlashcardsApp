using FlashcardsApp.Interfaces;
using FlashcardsApp.Models;

namespace FlashcardsApp
{
    /// <summary>
    /// Database repository containing all models, which are responsible for database queries in SQL.
    /// Not supporting AuthModel as it's put on Entity Framework.
    /// </summary>
    public class FlashcardsRepository : IFlashcardsRepository
    {
        /// <summary>
        /// Interface of all queries concerning deck management
        /// </summary>
        public IDeckModel _deckModel { get; set; }

        /// <summary>
        /// Interface of all queries concerning learning scheme management
        /// </summary>
        public ILearnModel _learnModel { get; set; }

        /// <summary>
        /// Interface of all queries concerning user management
        /// </summary>
        public IUserModel _userModel { get; set; }

        /// <summary>
        /// Creates all models for controllers to use.
        /// </summary>
        /// <param name="connectionString">Connection string to database, stored in appsettings.json</param>
        public FlashcardsRepository(string connectionString) {
            _deckModel = new DeckModel(connectionString);
            _learnModel = new LearnModel(connectionString);
            _userModel = new UserModel(connectionString);
        }

    }
}
