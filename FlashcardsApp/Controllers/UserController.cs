using FlashcardsApp.Dtos;
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

        [HttpGet("owned-decks"), Authorize]
        public async Task<ActionResult<User>> GetOwnedDecks(string id)
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

                    string json = JsonSerializer.Serialize(decks);
                    reader.Close();
                    return Ok(json);
                }
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("acquire"), Authorize]
        public async Task<ActionResult<User>> Acquire(string id)
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
