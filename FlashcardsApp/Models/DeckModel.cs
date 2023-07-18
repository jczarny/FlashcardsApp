using FlashcardsApp.Dtos;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FlashcardsApp.Models
{
    public class DeckModel
    {
        public static string? _connectionString { get; set; }
        public DeckModel(string connectionString)
        {
            _connectionString = connectionString;
        }

        /* Creates deck with userId as his owner.
         * Returns true if succeeded.
         */
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

        /*
         * Get information about deck.
         * userId is used to check if he's the owner or not.
         */
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
        
        /*
         * Get deck's cards.
         */
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

        /*
         * Get all decks that are marked as public.
         */
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
    
        /*
         *  Adds card to deck.
         *  Returns Id of added card, or -1 if user does not own deck to which he want add card to.
         */
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
    
        /*
         * Deletes card by its id.
         */
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

        /*
         * Deletes deck if userId is its owner, or just removes it from acquired if its not his.
         * Cards and RevisionLogs are cascaded automatically.
         */
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

        /*
         * Publish deck if its possible.
         */
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
