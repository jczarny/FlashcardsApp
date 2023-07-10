using FlashcardsApp.Dtos;
using FlashcardsApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace FlashcardsApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class DeckController : ControllerBase
    {
        private readonly FlashcardsContext _context;
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public DeckController(FlashcardsContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("SQLServer")!;
        }

        [HttpPost("create"), Authorize]
        public async Task<ActionResult<DeckDto>> Create(NewDeckDto deck)
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

        [HttpGet, Authorize]
        public async Task<ActionResult<DeckDto>> GetDeck(string deckId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(
                    "select * from Decks where id=" + deckId,
                    connection);

                await connection.OpenAsync();

                SqlDataReader reader = command.ExecuteReader();
                reader.Read();
                DeckDto deck = new DeckDto{
                    Id = reader.GetInt32("Id"),
                    CreatorId = reader.GetInt32("CreatorId"),
                    Title = reader.GetString("Title"),
                    Description = reader.GetString("Description"),
                    isPrivate = reader.GetBoolean("isPrivate")
                };
                reader.Close();

                SqlCommand cmd = new SqlCommand(
                    "select * from cards where Deckid=" + deckId, connection);

                reader = cmd.ExecuteReader();

                while(reader.Read()) {
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

        [HttpGet("getPublic"), Authorize]
        public async Task<ActionResult<DeckDto>> GetPublicDecks()
        {
            List<DeckDto> decks = new List<DeckDto>();
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

        [HttpPost("addcard"), Authorize]
        public async Task<ActionResult<DeckDto>> AddCard(CardDto card)
        {
            string json = "";
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

        [HttpDelete("card"), Authorize]
        public async Task<ActionResult<DeckDto>> DeleteCard(string id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand($"delete from cards where id = {Int32.Parse(id)}", connection);
                await connection.OpenAsync();
                cmd.ExecuteReader();

                return Ok();
            }
        }

        [HttpDelete, Authorize]
        public async Task<ActionResult<DeckDto>> DeleteDeck(string id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand($"delete from decks where id = {Int32.Parse(id)}", connection);
                await connection.OpenAsync();
                cmd.ExecuteReader();

                return Ok();
            }
        }

        [HttpPatch("publish"), Authorize]
        public async Task<ActionResult<DeckDto>> Publish(string id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("update decks set isPrivate=0 where Id=" + id, connection);
                await connection.OpenAsync();
                cmd.ExecuteReader();

                return Ok();
            }
        }
    }
}
