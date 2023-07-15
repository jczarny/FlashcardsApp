using FlashcardsApp.Dtos;
using FlashcardsApp.Entities;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace FlashcardsApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class UserController : ControllerBase
    {
        private readonly string _connectionString;

        public UserController(IConfiguration configuration) {

            _connectionString = configuration.GetConnectionString("SQLServer")!;
        }

        /*
         * Get dictionary of owned decks and amount of cards to revise today.
         */
        [HttpGet("owned-decks"), Authorize]
        public async Task<ActionResult<List<DeckDto>>> GetOwnedDecks(string id)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand command = new SqlCommand(
                        "select * from UserDecks as ud join Decks as d on ud.DeckId = d.Id where userId=" + Int32.Parse(id),
                        connection);

                    await connection.OpenAsync();

                    SqlDataReader reader = command.ExecuteReader();

                    List<DeckDto> decks = new List<DeckDto>();
                    while (reader.Read())
                    {
                        decks.Add(new DeckDto
                        {
                            Id = reader.GetInt32("DeckId"),
                            CreatorId = reader.GetInt32("CreatorId"),
                            Title = reader.GetString("Title"),
                            Description = reader.GetString("Description")
                        });
                    }
                    reader.Close();

                    Learn learnModel = new();
                    Dictionary<int, int> deckIdAmountPairs = await learnModel.GetReviseCardAmount(Int32.Parse(id), _connectionString);

                    for(int i = 0; i<decks.Count; i++)
                    {
                        bool isDeckInLog = deckIdAmountPairs.TryGetValue(decks[i].Id, out int amount);
                        if (isDeckInLog)
                            decks[i].CardsToRevise = amount;
                        else
                            decks[i].CardsToRevise = 0;
                    }

                    string json = JsonSerializer.Serialize(decks);
                    return Ok(json);
                }
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("acquire"), Authorize]
        public async Task<IActionResult> Acquire(string id)
        {
            try
            {
                string userId = Request.Headers["userId"].ToString();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    SqlCommand cmd = new SqlCommand("insert into UserDecks(UserId, DeckId)" +
                        $" values ({Int32.Parse(userId)}, {Int32.Parse(id)})", connection);
                    await connection.OpenAsync();
                    cmd.ExecuteReader();

                    return Ok();
                }
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
