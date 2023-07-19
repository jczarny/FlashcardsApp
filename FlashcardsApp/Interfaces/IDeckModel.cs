using FlashcardsApp.Dtos;

namespace FlashcardsApp.Interfaces
{
    /// <summary>
    /// Model servicing all queries regarding deck management.
    /// </summary>
    public interface IDeckModel
    {
        /// <summary>
        /// Creates deck with userId as his owner.
        /// </summary>
        /// <param name="deck">Object containing new deck's title and description.</param>
        /// <param name="userId">User creating the deck.</param>
        /// <returns>Returns True if succeedes.</returns>
        public Task<bool> CreateDeck(NewDeckDto deck, int userId);
        /// <summary>
        /// Gets information about deck.
        /// </summary>
        /// <param name="deckId">Id of deck user want to get information about.</param>
        /// <param name="userId">Id of user requesting this info, to check whether he's possessing the deck or not.</param>
        /// <returns>Returns DeckDto with interesting for user's data.</returns>
        public Task<DeckDto> GetDeckInfo(int deckId, int userId);
        /// <summary>
        /// Get deck's cards.
        /// </summary>
        /// <param name="deckId">Id of deck we want to grab cards from.</param>
        /// <returns>Returns list of cards as CardDto objects.</returns>
        public Task<List<CardDto>> GetDeckCards(int deckId);
        /// <summary>
        /// Gets all decks that are marked as public.
        /// </summary>
        /// <param name="userId">User Id, required to check whether user is an owner of deck or not.</param>
        /// <returns>Returns list of decks as DeckDto objects.</returns>
        public Task<List<DeckDto>> GetPublicDecks(int userId);
        /// <summary>
        /// Adds card to deck.
        /// </summary>
        /// <param name="card">Card object with all necessary data to record one in database.</param>
        /// <param name="userId">Id of user to check whether user has rights to add card.</param>
        /// <returns>Returns Id of added card, or -1 if user does not own deck to which he want add card to.</returns>
        public Task<string> AddCardToDeck(CardDto card, int userId);
        /// <summary>
        /// Deletes card by its id.
        /// </summary>
        /// <param name="cardId">Id of card to be deleted.</param>
        /// <param name="userId">Id of user to check whether user has rights to delete card.</param>
        /// <returns>Returns True if succeeded.</returns>
        public Task<bool> DeleteCardFromDeck(int cardId, int userId);
        /// <summary>
        /// Deletes deck if userId is its owner, or just removes it from acquired if its not his. \
        /// Cards and RevisionLogs are cascaded automatically.
        /// </summary>
        /// <param name="userId">Id of user to check whether user has rights to delete deck.</param>
        /// <param name="deckId"></param>
        /// <returns>Returns True if succeeded.</returns>
        public Task<bool> DeleteDeck(int userId, int deckId);
        /// <summary>
        /// Publishes deck.
        /// </summary>
        /// <param name="deckId">Id of deck to be published.</param>
        /// <param name="userId">Id of user to check wheter user has rights for such decision.</param>
        /// <returns>Returns True if succeeded.</returns>
        /// <exception cref="ArgumentException">Raised if User is not creator of this deck.</exception>
        public Task<bool> PublishDeck(int deckId, int userId);

    }
}
