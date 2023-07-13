using FlashcardsApp.Dtos;
using FlashcardsApp.Entities;
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

        [HttpPost("rate"), Authorize]
        public async Task<IActionResult> Rate()
        {
            return Ok();
        }

        [HttpGet, Authorize]
        public async Task<ActionResult<List<CardDto>>> GetLearningCards()
        {
            List<CardDto> cards = new List<CardDto>();
            CardDto card = new CardDto
            {
                Id = 1,
                DeckId = 1,
                Front = "siema",
                Reverse = "hello sup",
                Description = "powitanie"
            };
            cards.Add(card);
            cards.Add(card);
            cards.Add(card);
            cards.Add(card);
            cards.Add(card);

            string json = JsonSerializer.Serialize(cards);

            return Ok(json);
        }
    }
}
