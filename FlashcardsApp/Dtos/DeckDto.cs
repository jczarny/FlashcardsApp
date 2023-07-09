namespace FlashcardsApp.Dtos
{
    public class DeckDto
    {
        public int Id { get; set; }
        public int CreatorId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<CardDto> CardDtos { get; set; } = new List<CardDto>();
    }
}
