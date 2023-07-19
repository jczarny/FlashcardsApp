namespace FlashcardsApp.Dtos
{
    public class DeckDto
    { 
        // Id of deck in database
        public int Id { get; set; }
        // Amount of cards to revise by a certain user (unrequired)
        public int? CardsToRevise { get; set; }
        // Is a certain user owner of this deck or did he acquire it in deck browser
        public bool IsOwner { get; set; }
        // Username of creator of deck used shown in deck browser
        public string CreatorName { get; set; } = string.Empty;
        // Deck title
        public string Title { get; set; } = string.Empty;
        // Deck description
        public string Description { get; set; } = string.Empty;
        // Is deck private or public
        public bool isPrivate { get; set; } = false;
        // List of cards belonging to this deck
        public List<CardDto> CardDtos { get; set; } = new List<CardDto>();
    }
}
