using FlashcardsApp.Dtos;
using FlashcardsApp.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FlashcardsApp.Models
{
    /// <summary>
    /// Model servicing all queries regarding deck management.
    /// </summary>
    public class DeckModel : IDeckModel
    {
        /// <summary>
        /// Connection string to sql server.
        /// </summary>
        public static string? _connectionString { get; set; }
        /// <summary>
        /// Inject connectionString required for communication with sql server.
        /// </summary>
        /// <param name="connectionString">connectionString from appsettings.json file.</param>
        public DeckModel(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Creates deck with userId as his owner.
        /// </summary>
        /// <param name="deck">Object containing new deck's title and description.</param>
        /// <param name="userId">User creating the deck.</param>
        /// <returns>Returns True if succeedes.</returns>
        public async Task<bool> CreateDeck(NewDeckDto deck, int userId)
        {
            try
            {
                // Using stored procedure, which adds deck record to Decks table
                // and also to UserDecks to set this deck as owned by this user.
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("spDeck_Create", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@CreatorId", userId));
                    cmd.Parameters.Add(new SqlParameter("@Title", deck.Title));
                    cmd.Parameters.Add(new SqlParameter("@Description", deck.Description));

                    await connection.OpenAsync();
                    cmd.ExecuteReader();
                }
                return true;
            } catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets information about deck.
        /// </summary>
        /// <param name="deckId">Id of deck user want to get information about.</param>
        /// <param name="userId">Id of user requesting this info, to check whether he's possessing the deck or not.</param>
        /// <returns>Returns DeckDto with interesting for user's data.</returns>
        /// <exception cref="ArgumentException">Raised if user does not possess such deck.</exception>
        public async Task<DeckDto> GetDeckInfo(int deckId, int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(
                        $"select * from Decks d join UserDecks ud on d.id = ud.DeckId where d.id={deckId} and ud.UserId = {userId}",
                        connection);

                    await connection.OpenAsync();
                    SqlDataReader reader = cmd.ExecuteReader();

                    reader.Read();
                    if (reader.IsDBNull("Id"))
                        throw new ArgumentException("This user does not have such deck");

                    DeckDto deck = new DeckDto
                    {
                        Id = reader.GetInt32("Id"),
                        IsOwner = reader.GetInt32("CreatorId") == userId ? true : false,
                        Title = reader.GetString("Title"),
                        Description = reader.GetString("Description"),
                        isPrivate = reader.GetBoolean("isPrivate")
                    };

                    reader.Close();
                    return deck;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get deck's cards.
        /// </summary>
        /// <param name="deckId">Id of deck we want to grab cards from.</param>
        /// <returns>Returns list of cards as CardDto objects.</returns>
        public async Task<List<CardDto>> GetDeckCards(int deckId)
        {
            List<CardDto> cards = new();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand(
                            "select * from cards where Deckid=" + deckId, connection);
                    await connection.OpenAsync();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        CardDto card = new CardDto
                        {
                            Id = reader.GetInt32("Id"),
                            Front = reader.GetString("Front"),
                            Reverse = reader.GetString("Reverse"),
                            Description = reader.GetString("Description")
                        };
                        cards.Add(card);
                    }

                    reader.Close();
                    return cards;
                }
            } catch
            {
                throw;
            }
        }

        /// <summary>
        /// Gets all decks that are marked as public.
        /// </summary>
        /// <param name="userId">User Id, required to check whether user is an owner of deck or not.</param>
        /// <returns>Returns list of decks as DeckDto objects.</returns>
        public async Task<List<DeckDto>> GetPublicDecks(int userId)
        {
            List<DeckDto> decks = new List<DeckDto>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand("select * from Decks d join Users u on d.CreatorId = u.Id where d.isPrivate=0", connection);
                    await connection.OpenAsync();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        decks.Add(new DeckDto
                        {
                            Id = reader.GetInt32("Id"),
                            IsOwner = reader.GetInt32("CreatorId") == userId ? true : false,
                            CreatorName = reader.GetString("Username"),
                            Title = reader.GetString("Title"),
                            Description = reader.GetString("Description")
                        });
                    }
                    reader.Close();
                    return decks;
                }
            } catch { throw; }
        }

        /// <summary>
        /// Adds card to deck.
        /// </summary>
        /// <param name="card">Card object with all necessary data to record one in database.</param>
        /// <param name="userId">Id of user to check whether user has rights to add card.</param>
        /// <returns>Returns Id of added card, or -1 if user does not own deck to which he want add card to.</returns>
        public async Task<string> AddCardToDeck(CardDto card, int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCard_Add", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UserId", userId));
                    cmd.Parameters.Add(new SqlParameter("@DeckId", card.DeckId));
                    cmd.Parameters.Add(new SqlParameter("@Front", card.Front));
                    cmd.Parameters.Add(new SqlParameter("@Reverse", card.Reverse));
                    cmd.Parameters.Add(new SqlParameter("@Description", card.Description));
                    var p1 = cmd.Parameters.Add(new SqlParameter("@Id", card.Id));
                    p1.Direction = ParameterDirection.Output;
                    await connection.OpenAsync();

                    using (var rdr = cmd.ExecuteReader())
                    {
                        return p1.Value.ToString() ?? "-1";
                    }
                }
            } catch
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes card by its id.
        /// </summary>
        /// <param name="cardId">Id of card to be deleted.</param>
        /// <param name="userId">Id of user to check whether user has rights to delete card.</param>
        /// <returns>Returns True if succeeded.</returns>
        /// <exception cref="ArgumentException">Raised if user does not possess such deck.</exception>
        public async Task<bool> DeleteCardFromDeck(int cardId, int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand($"select * from cards c join decks d on c.DeckId = d.id where CreatorId={userId} and c.Id={cardId};", connection);
                    await connection.OpenAsync();
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    if (reader.IsDBNull("Front"))
                        throw new ArgumentException("User does not own this deck");
                    reader.Close();

                    cmd = new SqlCommand($"delete from cards where id = {cardId}", connection);
                    cmd.ExecuteReader();

                    return true;
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// Deletes deck if userId is its owner, or just removes it from acquired if its not his. \
        /// Cards and RevisionLogs are cascaded automatically.
        /// </summary>
        /// <param name="userId">Id of user to check whether user has rights to delete deck.</param>
        /// <param name="deckId"></param>
        /// <returns>Returns True if succeeded.</returns>
        public async Task<bool> DeleteDeck(int userId, int deckId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spDeck_Delete", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@DeckId", deckId));
                    cmd.Parameters.Add(new SqlParameter("@UserId", userId));

                    await connection.OpenAsync();
                    cmd.ExecuteReader();

                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Publishes deck.
        /// </summary>
        /// <param name="deckId">Id of deck to be published.</param>
        /// <param name="userId">Id of user to check wheter user has rights for such decision.</param>
        /// <returns>Returns True if succeeded.</returns>
        /// <exception cref="ArgumentException">Raised if User is not creator of this deck.</exception>
        public async Task<bool> PublishDeck(int deckId, int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand($"select * from decks where CreatorId={userId}", connection);
                    await connection.OpenAsync();
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    if (reader.IsDBNull("Id"))
                        throw new ArgumentException("User is not creator of this deck");
                    reader.Close();

                    cmd = new SqlCommand("update decks set isPrivate=0 where Id=" + deckId, connection);
                    cmd.ExecuteReader();

                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
