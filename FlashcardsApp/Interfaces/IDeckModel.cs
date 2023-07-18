using FlashcardsApp.Dtos;

namespace FlashcardsApp.Interfaces
{
    public interface IDeckModel
    {
        public Task<bool> CreateDeck(NewDeckDto deck, int userId);
        public Task<DeckDto> GetDeckInfo(int deckId, int userId);
        public Task<List<CardDto>> GetDeckCards(int deckId);
        public Task<List<DeckDto>> GetPublicDecks(int userId);
        public Task<string> AddCardToDeck(CardDto card, int userId);
        public Task<bool> DeleteCardFromDeck(int cardId, int userId);
        public Task<bool> DeleteDeck(int userId, int deckId);
        public Task<bool> PublishDeck(int deckId, int userId);

    }
}
