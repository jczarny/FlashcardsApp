namespace FlashcardsApp.Models
{
    public class Learn
    {
        public int CardId { get; set; }
        public int? UserId { get; set; }
        public int DeckId { get; set; }
        public DateTime? Date { get; set; }
        public int? Ease { get; set; }
        public int? Stage { get; set; }
        public string Front { get; set; } = string.Empty;
        public string Reverse { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
