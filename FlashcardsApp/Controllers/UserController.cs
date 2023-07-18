using FlashcardsApp.Dtos;
using FlashcardsApp.Interfaces;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FlashcardsApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class UserController : ControllerBase
    {
        private readonly IUserModel _userModel;
        private readonly string _connectionString;
        public UserController(IConfiguration configuration, IFlashcardsRepository repo)
        {

            _connectionString = configuration.GetConnectionString("SQLServer")!;
            _userModel = repo._userModel;
        }

        /*
         * Get dictionary of owned decks and amount of cards to revise today.
         */
        [HttpGet("owned-decks"), Authorize]
        public async Task<ActionResult<List<DeckDto>>> GetOwnedDecks()
        {

            try
            {
                // Validation
                var userIdString = Request.Cookies["userId"];
                UserIdToken.ParseTokenToInt(userIdString, out int userId);

                // Get decks information
                List<DeckDto> decks = await _userModel.GetUsersDeckInfo(userId);

                // Get amount of cards to revise from each deck
                LearnModel learnModel = new LearnModel(_connectionString);
                Dictionary<int, int> deckIdAmountPairs = await learnModel.GetReviseCardAmount(userId);

                // Merge the deck info with amount of cards to revise
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

        /*
         * Acquire public deck from deck browser.
         */
        [HttpPost("acquire"), Authorize]
        public async Task<IActionResult> Acquire([FromQuery] string id)
        {
            // Validation of deckId and userId 
            if (id == null)
            {
                return BadRequest("No deck specified");
            }
            bool isIdInt = int.TryParse(id, out int deckId);
            if (!isIdInt)
            {
                return BadRequest("Corrupted query");
            }

            try
            {
                var userIdString = Request.Cookies["userId"];
                UserIdToken.ParseTokenToInt(userIdString, out int userId);

                // Note in database the acquisition
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
