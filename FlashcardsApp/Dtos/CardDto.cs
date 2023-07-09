namespace FlashcardsApp.Dtos
{
    public class CardDto
    {
        public int? Id { get; set; }
        public int DeckId { get; set; }
        public string Front { get; set; } = string.Empty;
        public string Reverse { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
