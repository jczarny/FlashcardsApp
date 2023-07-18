namespace FlashcardsApp.Dtos
{
    // Used while learning a card
    public class LearnDto
    {
        // Id of card in database
        public int CardId { get; set; }
        // Id of deck in database
        public int DeckId { get; set; }
        // What was the response given by user (Try again/Hard/Medium/Easy)
        public int Response { get; set; }
    }
}
