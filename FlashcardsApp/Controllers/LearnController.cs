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
    /// Class <c>LearnController</c> manages learning process by feeding user with new cards,
    /// and evaluating learning progress.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class LearnController : ControllerBase
    {
        /// <summary>
        /// Model containing database queries concerning learning process.
        /// </summary>
        private readonly ILearnModel _learnModel;

        /// <summary>
        /// Constructor injecting dependency of: \
        /// - repository for commissioning database calls, \
        /// - configuration for acquiring connectionString which might come in handy.
        /// </summary>
        /// <param name="configuration">Configuration stored in appsettings.json</param>
        /// <param name="repo">Repository containing all database queries.</param>
        public LearnController(IConfiguration configuration, IFlashcardsRepository repo)
        {
            _learnModel = repo._learnModel;
        }

        /// <summary>
        /// Evaluate user's knowledge about given card. \
        /// More about evaluation in learnModel.
        /// </summary>
        /// <param name="learnData">object with id of learning card, id of deck this card belongs to, and 
        /// response the user gave about this card (his proficiency). </param>
        /// <returns>Returns Ok() if evaluation went correctly, otherwise BadRequest(400)</returns>
        [HttpPost("evaluate"), Authorize]
        public async Task<IActionResult> Evaluate([FromBody] LearnDto learnData)
        {

            try
            {
                var userIdString = Request.Cookies["userId"];
                UserIdToken.ParseTokenToInt(userIdString, out int userId);

                bool isSuccess = await _learnModel.EvaluateResult(
                    userId, learnData.CardId, learnData.DeckId, learnData.Response);
                if (isSuccess)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            } catch
            {
                return BadRequest("Unknown error in card knowledge evaluation");
            }
        }

        /// <summary>
        /// Get some relevant cards from deck to study from.
        /// </summary>
        /// <param name="deckId">Id of currently learned deck.</param>
        /// <param name="amount"> defines how many cards to get from deck. \
        /// The greater the number the less amount of GetLearningCards() calls, but its response size increases. </param>
        /// <returns>Returns list of cards, which user will study from.</returns>
        [HttpGet, Authorize]
        public async Task<ActionResult<List<CardDto>>> GetLearningCards([FromQuery] int deckId, [FromQuery] int amount)
        {
            // Validation
            var userIdString = Request.Cookies["userId"];
            int userId;
            try
            {
                UserIdToken.ParseTokenToInt(userIdString, out userId);

                // Get cards from database
                List<CardDto> cards = await _learnModel.GetLearningCards(userId, deckId, amount);

                string json = JsonSerializer.Serialize(cards);

                // If no suitable cards were found, respond with empty Ok
                if (cards.Count != 0)
                    return Ok(json);
                else
                    return Ok("");

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    
    }
}
