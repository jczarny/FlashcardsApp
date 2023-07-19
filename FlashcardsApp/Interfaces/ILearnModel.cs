using FlashcardsApp.Dtos;

namespace FlashcardsApp.Interfaces
{
    /// <summary>
    /// Model servicing all queries regarding learning process management.
    /// </summary>
    public interface ILearnModel
    {
        /// <summary>
        /// Function <c>GetLearningCards</c> gets learning cards by following schema: \
        /// Firstly, check if some cards require revision (if date in RevisionLog is past today's one). \
        /// Secondly, get new cards that have never been seen (exist in Deck, but not in RevisionLog). \
        /// At last, if we run out of cards - signal it properly.
        /// </summary>
        /// <param name="userId">Id of user currently is learning.</param>
        /// <param name="deckId">Id of deck user's is currently learning from.</param>
        /// <param name="cardAmount">Amount of cards we want to grab from database, the higher the value the less
        /// calls to database, but larger response sizes.</param>
        /// <returns>Returns list of cards most urgent to revise/learn.</returns>
        public Task<List<CardDto>> GetLearningCards(int userId, int deckId, int cardAmount);
        /// <summary>
        /// Function <c>EvaluateResult</c> evaluate user's knowledge depending on its response now and historical responses.
        /// </summary>
        /// <param name="userId">Id of user currently is learning.</param>
        /// <param name="cardId">Id of card user's is currently learning from.</param>
        /// <param name="deckId">Id of deck user's is currently learning from.</param>
        /// <param name="response">Response the user gave grading his proficiency about this card. \
        /// (Try again (1), Hard (2), ... )</param>
        /// <returns>Returns 1 if it evaluates successfully</returns>
        public Task<bool> EvaluateResult(int userId, int cardId, int deckId, int response);
        /// <summary>
        /// Get amount of cards that need to be revised today. \
        /// Shown in home's deck card.
        /// </summary>
        /// <param name="userId">Id of user.</param>
        /// <returns>Returns dictionary of deckId and amount of cards to revise for this particular user.</returns>
        public Task<Dictionary<int, int>> GetReviseCardAmount(int userId);
    }
}
