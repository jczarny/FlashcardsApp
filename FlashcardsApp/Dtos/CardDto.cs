namespace FlashcardsApp.Dtos
{
    public class CardDto
    {
        // Id of card in database
        public int? Id { get; set; }
        // Id of deck the cards belongs to
        public int DeckId { get; set; }
        // Front text of card
        public string Front { get; set; } = string.Empty;
        // Reverse text of card
        public string Reverse { get; set; } = string.Empty;
        // Description of card
        public string Description { get; set; } = string.Empty;
    }
}
