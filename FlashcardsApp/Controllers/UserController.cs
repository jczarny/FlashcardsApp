using FlashcardsApp.Dtos;
using FlashcardsApp.Interfaces;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FlashcardsApp.Controllers
{
    /// <summary>
    /// Class <c>UserController</c> filled with endpoints concerening user-to-deck management.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Model containing database queries concerning user management.
        /// </summary>
        private readonly IUserModel _userModel;
        /// <summary>
        /// Model containing database queries concerning learning process management.
        /// </summary>
        private readonly ILearnModel _learnModel;
        private readonly string _connectionString;

        /// <summary>
        /// Constructor injecting dependency of: \
        /// - repository for commissioning database calls, \
        /// - configuration for acquiring connectionString which might come in handy.
        /// </summary>
        /// <param name="configuration">Configuration stored in appsettings.json</param>
        /// <param name="repo">Repository containing all database queries.</param>
        public UserController(IConfiguration configuration, IFlashcardsRepository repo)
        {

            _connectionString = configuration.GetConnectionString("SQLServer")!;
            _userModel = repo._userModel;
            _learnModel = repo._learnModel;
        }

        /// <summary>
        /// Get list of owned decks with amount of cards to revise today.
        /// </summary>
        /// <returns>Returns jsonSerialized list of decks</returns>
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
                Dictionary<int, int> deckIdAmountPairs = await _learnModel.GetReviseCardAmount(userId);

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

        /// <summary>
        /// Acquire public deck from deck browser.
        /// </summary>
        /// <param name="id">Id of deck to be acquired.</param>
        /// <returns>Returns Ok() if success, BadRequest(400) if something went wrong.</returns>
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
