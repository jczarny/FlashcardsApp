using FlashcardsApp.Dtos;
using FlashcardsApp.Entities;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace FlashcardsApp.Models
{
    public class DeckModel
    {
        public static string? _connectionString { get; set; }
        public DeckModel(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<bool> CreateDeck(NewDeckDto deck, int userId)
        {
            try
            {
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

        // Get the deck info with checking if user really owns this deck
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
    
        public async Task<object> AddCardToDeck(CardDto card)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("spCard_Add", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@DeckId", card.DeckId));
                    cmd.Parameters.Add(new SqlParameter("@Front", card.Front));
                    cmd.Parameters.Add(new SqlParameter("@Reverse", card.Reverse));
                    cmd.Parameters.Add(new SqlParameter("@Description", card.Description));
                    var p1 = cmd.Parameters.Add(new SqlParameter("@Id", card.Id));
                    p1.Direction = ParameterDirection.Output;
                    await connection.OpenAsync();

                    using (var rdr = cmd.ExecuteReader())
                    {
                        return p1.Value;
                    }
                }
            } catch
            {
                throw;
            }
        }
    
        public async Task<bool> DeleteCardFromDeck(int cardId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand($"delete from cards where id = {cardId}", connection);
                    await connection.OpenAsync();
                    cmd.ExecuteReader();

                    return true;
                }
            }
            catch { throw; }
        }

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

        public async Task<bool> PublishDeck(int deckId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("update decks set isPrivate=0 where Id=" + deckId, connection);
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
    }
}
