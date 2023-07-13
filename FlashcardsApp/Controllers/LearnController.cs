using FlashcardsApp.Dtos;
using FlashcardsApp.Entities;
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

        public LearnController(IConfiguration configuration)
        {

            _connectionString = configuration.GetConnectionString("SQLServer")!;
        }

        [HttpPost("evaluate"), Authorize]
        public async Task<IActionResult> Evaluate([FromBody] LearnDto learnData)
        {
            string userId = Request.Headers["userId"].ToString();

            Learn learnModel = new Learn();
            int result = await learnModel.EvaluateResult(
                Int32.Parse(userId), learnData.CardId, learnData.DeckId, learnData.Response, _connectionString);
            if (result == 1)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet, Authorize]
        public async Task<ActionResult<List<CardDto>>> GetLearningCards(int deckId, int amount)
        {
            string userId = Request.Headers["userId"].ToString();

            Learn learnModel = new Learn();
            List<CardDto> cards = await learnModel.GetLearningCards(Int32.Parse(userId), deckId, amount, _connectionString);

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
