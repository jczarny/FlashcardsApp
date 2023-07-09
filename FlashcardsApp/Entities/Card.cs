namespace FlashcardsApp.Entities
{
    public class Card
    {
        public int Id { get; set; }
        public int DeckId { get; set; }
        public Deck? Deck { get; set; }
        public string Front { get; set; } = string.Empty;
        public string Reverse { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IEnumerable<RevisionLog>? RevisionLogs { get; set; }
    }
}
