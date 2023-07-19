namespace FlashcardsApp.Entities
{
    /// <summary>
    /// Entity depicting card data. Card can't exist on its own, only within certain deck.
    /// </summary>
    public class Card
    {
        /// <summary>
        /// Card's primary key.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Deck this card belongs to.
        /// </summary>
        public int DeckId { get; set; }
        /// <summary>
        /// Used in Entity framework modeling.
        /// </summary>
        public Deck? Deck { get; set; }
        /// <summary>
        /// Front text of card.
        /// </summary>
        public string Front { get; set; } = string.Empty;
        /// <summary>
        /// Reverse text of card.
        /// </summary>
        public string Reverse { get; set; } = string.Empty;
        /// <summary>
        /// Description text of card.
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Used in Entity framework modeling.
        /// </summary>
        public IEnumerable<RevisionLog>? RevisionLogs { get; set; }
    }
}
