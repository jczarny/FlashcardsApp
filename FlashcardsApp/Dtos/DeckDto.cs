namespace FlashcardsApp.Dtos
{
    /// <summary>
    /// Data Transfer Object between frontend and backend. Depicts basic information about deck.
    /// </summary>
    public class DeckDto
    { 
        /// <summary>
        /// Id of deck in database.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Amount of cards to revise by a certain user (unrequired).
        /// </summary>
        public int? CardsToRevise { get; set; }
        /// <summary>
        /// Answers a question: Is a certain user owner of this deck or did he acquire it in deck browser?
        /// </summary>
        public bool IsOwner { get; set; }
        /// <summary>
        /// Username of creator of deck used shown in deck browser.
        /// </summary>
        public string CreatorName { get; set; } = string.Empty;
        /// <summary>
        /// Deck title.
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// Deck description.
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// Is deck private or public.
        /// </summary>
        public bool isPrivate { get; set; } = false;
        /// <summary>
        /// List of cards belonging to this deck.
        /// </summary>
        public List<CardDto> CardDtos { get; set; } = new List<CardDto>();
    }
}
