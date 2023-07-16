using FlashcardsApp.Dtos;
using FlashcardsApp.Entities;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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

        [HttpPost("evaluate"), Authorize]
        public async Task<IActionResult> Evaluate([FromBody] LearnDto learnData)
        {
            string userIdString = Request.Headers["userId"].ToString();
            if(userIdString == null) {
                return BadRequest("No user specified");
            }
            bool isIdInt = int.TryParse(userIdString, out int userId);
            if(!isIdInt) {
                return BadRequest("Corrupted header");
            }

            try
            {
                int result = await _learnModel.EvaluateResult(
                    userId, learnData.CardId, learnData.DeckId, learnData.Response);
                return Ok();
            } catch
            {
                return BadRequest("Unknown error in card knowledge evaluation");
            }
        }

        [HttpGet, Authorize]
        public async Task<ActionResult<List<CardDto>>> GetLearningCards(int deckId, int amount)
        {
            string userIdString = Request.Headers["userId"].ToString();
            if (userIdString == null)
            {
                return BadRequest("No user specified");
            }
            bool isIdInt = int.TryParse(userIdString, out int userId);
            if (!isIdInt)
            {
                return BadRequest("Corrupted header");
            }

            List<CardDto> cards = await _learnModel.GetLearningCards(userId, deckId, amount);

            string json = JsonSerializer.Serialize(cards);

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
