using FlashcardsApp.Dtos;
using FlashcardsApp.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FlashcardsApp.Models
{
    public class UserModel : IUserModel
    {
        public readonly string _connectionString;

        public UserModel(string connectionString)
        {
            _connectionString = connectionString;
        }
    
        /*
         * Get information about all decks owned by particular user.
         */
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

        /*
         * Get deck which is said to be public.
         */
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
