namespace FlashcardsApp.Dtos
{
    /// <summary>
    /// Data Transfer Object between frontend and backend. Used in learning process.
    /// </summary>
    public class LearnDto
    {
        /// <summary>
        /// Id of currently learned card in database.
        /// </summary>
        public int CardId { get; set; }
        /// <summary>
        /// Id of currently learned deck in database.
        /// </summary>
        public int DeckId { get; set; }
        /// <summary>
        /// Answers what was the response given by user (Try again(1)/Hard(2)/Medium(3)/Easy(4)) \
        /// Depending on response, different next revision date will be set.
        /// </summary>
        public int Response { get; set; }
    }
}
