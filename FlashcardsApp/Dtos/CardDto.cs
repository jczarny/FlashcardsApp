namespace FlashcardsApp.Dtos
{
    /// <summary>
    /// Data Transfer Object between frontend and backend. Depicts basic information about card.
    /// </summary>
    public class CardDto
    {
        /// <summary>
        /// Id of card in database.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Id of deck the cards belongs to.
        /// </summary>
        public int DeckId { get; set; }
        /// <summary>
        /// Front text of card, from which user has to guess reverse.
        /// </summary>
        public string Front { get; set; } = string.Empty;
        /// <summary>
        /// Reverse text of card.
        /// </summary>
        public string Reverse { get; set; } = string.Empty;
        /// <summary>
        /// Description helping user understand the usage of term, for example word used in a sentence.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
