namespace FlashcardsApp.Models
{
    /// <summary>
    /// Class merging card data with metrics depicting learning process.
    /// </summary>
    public class Learn
    {
        /// <summary>
        /// Card's id in database.
        /// </summary>
        public int CardId { get; set; }
        /// <summary>
        /// Id of learning user.
        /// </summary>
        public int? UserId { get; set; }
        /// <summary>
        /// Id of deck in database.
        /// </summary>
        public int DeckId { get; set; }
        /// <summary>
        /// Date of presumably next revision of card.
        /// </summary>
        public DateTime? Date { get; set; }
        /// <summary>
        /// Value depicting this user's previous response about this card.
        /// </summary>
        public int? Ease { get; set; }
        /// <summary>
        /// Value depicting proficiency in this card.
        /// </summary>
        public int? Stage { get; set; }
        /// <summary>
        /// Card's front text.
        /// </summary>
        public string Front { get; set; } = string.Empty;
        /// <summary>
        /// Card's reverse text.
        /// </summary>
        public string Reverse { get; set; } = string.Empty;
        /// <summary>
        /// Card's description.
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
