namespace FlashcardsApp.Dtos
{
    public class NewDeckDto
    {
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
