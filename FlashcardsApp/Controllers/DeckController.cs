using FlashcardsApp.Dtos;
using FlashcardsApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace FlashcardsApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class DeckController : ControllerBase
    {
        private readonly string _connectionString;

        // Get db context, project config and connection string
        public DeckController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SQLServer")!;
        }

        // Create deck with given title and description
        [HttpPost("create"), Authorize]
        public async Task<IActionResult> CreateDeck([FromBody] NewDeckDto deck)
        {
            int result = ValidateNewDeckDto(deck);
            if (result == 0) return BadRequest();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("spDeck_Create", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@CreatorId", deck.UserId));
                    cmd.Parameters.Add(new SqlParameter("@Title", deck.Title));
                    cmd.Parameters.Add(new SqlParameter("@Description", deck.Description));

                    await connection.OpenAsync();
                    cmd.ExecuteReader();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get user's deck by its id
        [HttpGet, Authorize]
        public async Task<ActionResult<DeckDto>> GetDeck([FromQuery] string deckId)
        {
            string userId = Request.Headers["userId"].ToString();

            // Check if deckId is really an integer
            int number;
            bool isInt = int.TryParse(deckId, out number);
            if (!isInt)
            {
                return BadRequest();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    // Get the deck info with checking if user really owns this deck
                    SqlCommand command = new SqlCommand(
                        $"select * from Decks d join UserDecks ud on d.id = ud.DeckId where d.id={deckId} and ud.UserId = {userId}",
                        connection);

                    await connection.OpenAsync();

                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    DeckDto deck = new DeckDto
                    {
                        Id = reader.GetInt32("Id"),
                        CreatorId = reader.GetInt32("CreatorId"),
                        Title = reader.GetString("Title"),
                        Description = reader.GetString("Description"),
                        isPrivate = reader.GetBoolean("isPrivate")
                    };
                    reader.Close();

                    // Get deck's cards
                    SqlCommand cmd = new SqlCommand(
                        "select * from cards where Deckid=" + deckId, connection);

                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        CardDto card = new CardDto
                        {
                            Id = reader.GetInt32("Id"),
                            Front = reader.GetString("Front"),
                            Reverse = reader.GetString("Reverse"),
                            Description = reader.GetString("Description")
                        };
                        deck.CardDtos.Add(card);
                    }
                    string json = JsonSerializer.Serialize(deck);
                    reader.Close();
                    return Ok(json);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get all public decks within db
        [HttpGet("getPublic"), Authorize]
        public async Task<ActionResult<List<DeckDto>>> GetPublicDecks()
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
                            CreatorId = reader.GetInt32("CreatorId"),
                            CreatorName = reader.GetString("Username"),
                            Title = reader.GetString("Title"),
                            Description = reader.GetString("Description")
                        });
                    }

                    string json = JsonSerializer.Serialize(decks);
                    reader.Close();
                    return Ok(json);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Add card to a deck, return id of new card
        [HttpPost("addcard"), Authorize]
        public async Task<ActionResult<int>> AddCard(CardDto card)
        {
            int result = ValidateCardDto(card);
            if (result == 0)
                return BadRequest();

            string json = "";
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
                        json = JsonSerializer.Serialize(p1.Value);
                    }
                }
                return Ok(json);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Delete card from a deck
        [HttpDelete("card"), Authorize]
        public async Task<IActionResult> DeleteCard(string id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand($"delete from cards where id = {Int32.Parse(id)}", connection);
                    await connection.OpenAsync();
                    cmd.ExecuteReader();

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Delete deck
        [HttpDelete, Authorize]
        public async Task<IActionResult> DeleteDeck(string id)
        {
            string userId = Request.Headers["userId"].ToString();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {

                    SqlCommand cmd = new SqlCommand("spDeck_Delete", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@DeckId", Int32.Parse(id)));
                    cmd.Parameters.Add(new SqlParameter("@UserId", userId));

                    await connection.OpenAsync();
                    cmd.ExecuteReader();

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // Make deck public (available to use for everyone)
        [HttpPatch("publish"), Authorize]
        public async Task<IActionResult> PublishDeck(string id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("update decks set isPrivate=0 where Id=" + Int32.Parse(id), connection);
                    await connection.OpenAsync();
                    cmd.ExecuteReader();

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        private int ValidateCardDto(CardDto card)
        {
            string regex = "^[a-zA-Z0-9]([a-zA-Z0-9 ]){1,32}[a-zA-Z0-9]$";

            if (card == null) return 0;
            if (!Regex.IsMatch(card.Front, regex)) return 0;
            if (!Regex.IsMatch(card.Reverse, regex)) return 0;
            if (card.Description.Length != 0 && !Regex.IsMatch(card.Description, regex)) return 0;
            return 1;
        }

        private int ValidateNewDeckDto(NewDeckDto newDeck)
        {
            string regex = "^[a-zA-Z0-9]([a-zA-Z0-9 ]){4,32}[a-zA-Z0-9]$";

            if (newDeck == null) return 0;
            if (!Regex.IsMatch(newDeck.Title, regex)) return 0;

            return 1;
        }
    }
}
