using FlashcardsApp.Dtos;

namespace FlashcardsApp.Interfaces
{
    public interface IUserModel
    {
        public Task<List<DeckDto>> GetUsersDeckInfo(int userId);
        public Task<IResult> AcquirePublicDeck(int userId, int deckId);

    }
}
