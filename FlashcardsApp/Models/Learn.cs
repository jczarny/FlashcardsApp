namespace FlashcardsApp.Models
{
    /*
     * Class merging card data with metrics depicting learning process.
     */
    public class Learn
    {
        // Card id in database
        public int CardId { get; set; }
        // Learning user
        public int? UserId { get; set; }
        // Deck id in database
        public int DeckId { get; set; }
        // Date of presumably next revision of card
        public DateTime? Date { get; set; }
        // Value depicting this user's previous response about this card of cardId
        public int? Ease { get; set; }
        // Value depicting proficiency in this card
        public int? Stage { get; set; }
        // Card's front text
        public string Front { get; set; } = string.Empty;
        // Card's reverse text
        public string Reverse { get; set; } = string.Empty;
        // Card's description
        public string Description { get; set; } = string.Empty;
    }
}
