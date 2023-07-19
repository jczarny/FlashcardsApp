using FlashcardsApp.Dtos;
using FlashcardsApp.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FlashcardsApp.Models
{
    /// <summary>
    /// Model servicing all queries regarding user-to-deck management.
    /// </summary>
    public class UserModel : IUserModel
    {
        /// <summary>
        /// Connection string to sql server.
        /// </summary>
        public readonly string _connectionString;

        /// <summary>
        /// Inject connectionString required for communication with sql server.
        /// </summary>
        /// <param name="connectionString">connectionString from appsettings.json file.</param>
        public UserModel(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Gets information about all decks owned by particular user.
        /// </summary>
        /// <param name="userId">Id of user requesting such action.</param>
        /// <returns>Returns list of decks in DeckDto objects.</returns>
        public async Task<List<DeckDto>> GetUsersDeckInfo(int userId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "select * from UserDecks as ud join Decks as d on ud.DeckId = d.Id where userId=" + userId,
                    connection);

                await connection.OpenAsync();
                SqlDataReader reader = command.ExecuteReader();

                List<DeckDto> decks = new List<DeckDto>();
                while (reader.Read())
                {
                    
                    decks.Add(new DeckDto
                    {
                        Id = reader.GetInt32("DeckId"),
                        IsOwner = reader.GetInt32("CreatorId") == userId ? true : false,
                        Title = reader.GetString("Title"),
                        Description = reader.GetString("Description")
                    });
                }
                reader.Close();
                return decks;
            }
        }

        /// <summary>
        /// Acquires deck which is said to be public. (as writing a record to UserDecks table)
        /// </summary>
        /// <param name="userId">Id of interested user.</param>
        /// <param name="deckId">Id of deck the user's is interested in.</param>
        /// <returns>Returns Ok() if succeedes.</returns>
        public async Task<IResult> AcquirePublicDeck(int userId, int deckId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("insert into UserDecks(UserId, DeckId)" +
                        $" values ({userId}, {deckId})", connection);
                    await connection.OpenAsync();
                    cmd.ExecuteReader();

                    return Results.Ok();
                }
            } catch
            {
                throw;
            }
        }
    }
}
