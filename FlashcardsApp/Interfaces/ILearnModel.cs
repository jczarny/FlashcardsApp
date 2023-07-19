using FlashcardsApp.Dtos;

namespace FlashcardsApp.Interfaces
{
    public interface ILearnModel
    {
        public Task<List<CardDto>> GetLearningCards(int userId, int deckId, int cardAmount);
        public Task<bool> EvaluateResult(int userId, int cardId, int deckId, int response);
        public Task<Dictionary<int, int>> GetReviseCardAmount(int userId);
    }
}
