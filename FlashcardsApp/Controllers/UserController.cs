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
        private readonly UserModel _userModel;
        private readonly string _connectionString;
        public UserController(IConfiguration configuration)
        {

            _connectionString = configuration.GetConnectionString("SQLServer")!;
            _userModel = new UserModel(_connectionString);
        }

        /*
         * Get dictionary of owned decks and amount of cards to revise today.
         */
        [HttpGet("owned-decks"), Authorize]
        public async Task<ActionResult<List<DeckDto>>> GetOwnedDecks(string id)
        {
            if (id == null)
            {
                return BadRequest("No user specified");
            }
            bool isIdInt = int.TryParse(id, out int userId);
            if (!isIdInt)
            {
                return BadRequest("Corrupted header");
            }

            try
            {
                List<DeckDto> decks = await _userModel.GetUsersDeckInfo(userId);

                LearnModel learnModel = new LearnModel(_connectionString);
                Dictionary<int, int> deckIdAmountPairs = await learnModel.GetReviseCardAmount(userId);

                for (int i = 0; i < decks.Count; i++)
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("acquire"), Authorize]
        public async Task<IActionResult> Acquire(string id)
        {
            if (id == null)
            {
                return BadRequest("No deck specified");
            }
            bool isIdInt = int.TryParse(id, out int deckId);
            if (!isIdInt)
            {
                return BadRequest("Corrupted header");
            }

            string userIdString = Request.Headers["userId"].ToString();
            if (userIdString == null)
            {
                return BadRequest("No user specified");
            }
            isIdInt = int.TryParse(userIdString, out int userId);
            if (!isIdInt)
            {
                return BadRequest("Corrupted header");
            }

            try
            {
                await _userModel.AcquirePublicDeck(userId, deckId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
