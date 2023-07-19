using FlashcardsApp.Dtos;

namespace FlashcardsApp.Interfaces
{
    /// <summary>
    /// Model servicing all queries regarding user-to-deck management.
    /// </summary>
    public interface IUserModel
    {
        /// <summary>
        /// Gets information about all decks owned by particular user.
        /// </summary>
        /// <param name="userId">Id of user requesting such action.</param>
        /// <returns>Returns list of decks in DeckDto objects.</returns>
        public Task<List<DeckDto>> GetUsersDeckInfo(int userId);
        /// <summary>
        /// Acquires deck which is said to be public. (as writing a record to UserDecks table)
        /// </summary>
        /// <param name="userId">Id of interested user.</param>
        /// <param name="deckId">Id of deck the user's is interested in.</param>
        /// <returns>Returns Ok() if succeedes.</returns>
        public Task<IResult> AcquirePublicDeck(int userId, int deckId);

    }
}
