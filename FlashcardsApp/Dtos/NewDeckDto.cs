namespace FlashcardsApp.Dtos
{
    /// <summary>
    /// Data transfer object for creating new deck.
    /// </summary>
    public class NewDeckDto
    {
        /// <summary>
        /// Deck's title.
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// Deck's description.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
