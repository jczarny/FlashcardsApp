namespace FlashcardsApp.Interfaces
{
    /// <summary>
    /// Database repository containing all models, which are responsible for database queries in SQL.
    /// Not supporting AuthModel as it's put on Entity Framework.
    /// </summary>
    public interface IFlashcardsRepository
    {
        /// <summary>
        /// Interface of all queries concerning deck management
        /// </summary>
        public IDeckModel _deckModel { get; set; }
        /// <summary>
        /// Interface of all queries concerning learning scheme management
        /// </summary>
        public ILearnModel _learnModel { get; set; }
        /// <summary>
        /// Interface of all queries concerning user management
        /// </summary>
        public IUserModel _userModel { get; set; }
    }
}
