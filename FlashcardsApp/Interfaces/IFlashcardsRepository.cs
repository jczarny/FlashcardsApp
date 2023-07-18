namespace FlashcardsApp.Interfaces
{
    public interface IFlashcardsRepository
    {
        public IDeckModel _deckModel { get; set; }
        public ILearnModel _learnModel { get; set; }
        public IUserModel _userModel { get; set; }
    }
}
