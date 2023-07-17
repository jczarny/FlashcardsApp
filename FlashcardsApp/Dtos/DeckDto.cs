namespace FlashcardsApp.Dtos
{
    public class DeckDto
    {
        public int Id { get; set; }
        public int? CardsToRevise { get; set; }
        public bool IsOwner { get; set; }
        public string CreatorName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool isPrivate { get; set; } = false;
        public List<CardDto> CardDtos { get; set; } = new List<CardDto>();
    }
}
