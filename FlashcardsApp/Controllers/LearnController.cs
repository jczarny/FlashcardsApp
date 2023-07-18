using FlashcardsApp.Dtos;
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
    public class LearnController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly LearnModel _learnModel;
        public LearnController(IConfiguration configuration)
        {

            _connectionString = configuration.GetConnectionString("SQLServer")!;
            _learnModel = new LearnModel(_connectionString);
        }

        /*
         * Evaluate user's knowledge about the card. 
         * More about evaluation in learnModel.
         */
        [HttpPost("evaluate"), Authorize]
        public async Task<IActionResult> Evaluate([FromBody] LearnDto learnData)
        {

            try
            {
                var userIdString = Request.Cookies["userId"];
                UserIdToken.ParseTokenToInt(userIdString, out int userId);

                int result = await _learnModel.EvaluateResult(
                    userId, learnData.CardId, learnData.DeckId, learnData.Response);
                return Ok();
            } catch
            {
                return BadRequest("Unknown error in card knowledge evaluation");
            }
        }

        /*
         * Get some relevant cards from deck to study from.
         * amount defines how many cards to get from deck.
         * The greater the number the less amount of responses, but its size increases.
         */
        [HttpGet, Authorize]
        public async Task<ActionResult<List<CardDto>>> GetLearningCards([FromQuery] int deckId, [FromQuery] int amount)
        {
            // Validation
            var userIdString = Request.Cookies["userId"];
            int userId;
            try
            {
                UserIdToken.ParseTokenToInt(userIdString, out userId);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            // Get cards from database
            List<CardDto> cards = await _learnModel.GetLearningCards(userId, deckId, amount);

            string json = JsonSerializer.Serialize(cards);

            // If no suitable cards were found, respond with empty Ok
            if (cards.Count != 0)
            {
                return Ok(json);
            }
            else
            {
                return Ok();
            }
        }
    
    }
}
