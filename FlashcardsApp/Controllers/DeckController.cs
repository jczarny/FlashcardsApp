using FlashcardsApp.Dtos;
using FlashcardsApp.Entities;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace FlashcardsApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class DeckController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly DeckModel _deckModel;

        // Get db context, project config and connection string
        public DeckController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SQLServer")!;
            _deckModel = new DeckModel(_connectionString);
        }

        // Get user's deck by its id
        [HttpGet, Authorize]
        public async Task<ActionResult<DeckDto>> GetDeck([FromQuery] string deckId)
        {
            string userIdString = Request.Headers["userId"].ToString();

            bool isIdInt = int.TryParse(userIdString, out int userId);
            if (!isIdInt)
            {
                return BadRequest();
            }
            isIdInt = int.TryParse(deckId, out int deckIdInt);
            if (!isIdInt)
            {
                return BadRequest();
            }

            try
            {
                // Get the deck info with checking if user really owns this deck
                DeckDto deck = await _deckModel.GetDeckInfo(deckIdInt, userId);

                // Get deck's cards
                deck.CardDtos = await _deckModel.GetDeckCards(deckIdInt);

                string json = JsonSerializer.Serialize(deck);
                return Ok(json);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Create deck with given title and description
        [HttpPost("create"), Authorize]
        public async Task<IActionResult> CreateDeck([FromBody] NewDeckDto deck)
        {
            bool isValid = ValidateNewDeckDto(deck);
            if (isValid is false)
                return BadRequest();

            try
            {
                await _deckModel.CreateDeck(deck);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Get all public decks within db
        [HttpGet("getPublic"), Authorize]
        public async Task<ActionResult<List<DeckDto>>> GetPublicDecks()
        {
            try
            {
                List<DeckDto> decks = await _deckModel.GetPublicDecks();
                string json = JsonSerializer.Serialize(decks);
                return Ok(json);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Add card to a deck, return id of new card
        [HttpPost("addcard"), Authorize]
        public async Task<ActionResult<int>> AddCard(CardDto card)
        {
            bool result = ValidateCardDto(card);
            if (result is false)
                return BadRequest("Invalid card values");

            string json = "";
            try
            {
                var cardId = await _deckModel.AddCardToDeck(card);
                json = JsonSerializer.Serialize(cardId);
                return Ok(json);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Delete card from a deck
        [HttpDelete("card"), Authorize]
        public async Task<IActionResult> DeleteCard(string id)
        {
            bool isIdInt = int.TryParse(id, out int cardId);
            if (!isIdInt)
            {
                return BadRequest();
            }

            try
            {
                var result = await _deckModel.DeleteCardFromDeck(cardId);
                if (result == Results.Ok())
                    return Ok();
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Delete deck
        [HttpDelete, Authorize]
        public async Task<IActionResult> DeleteDeck(string id)
        {
            string userIdString = Request.Headers["userId"].ToString();
            bool isIdInt = int.TryParse(userIdString, out int userId);
            if (!isIdInt)
            {
                return BadRequest();
            }

            isIdInt = int.TryParse(id, out int deckId);
            if (!isIdInt)
            {
                return BadRequest();
            }

            try
            {
                var result = await _deckModel.DeleteDeck(userId, deckId);

                if(result == Results.Ok()) return Ok();
                else return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // Make deck public (available to use for everyone)
        [HttpPatch("publish"), Authorize]
        public async Task<IActionResult> PublishDeck(string id)
        {
            bool isIdInt = int.TryParse(id, out int deckId);
            if (!isIdInt)
            {
                return BadRequest();
            }

            try
            {
                var result = await _deckModel.PublishDeck(deckId);
                if (result == Results.Ok()) return Ok();
                else return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        private bool ValidateCardDto(CardDto card)
        {
            string regex = "^[a-zA-Z0-9]([a-zA-Z0-9 ]){1,32}[a-zA-Z0-9]$";

            if (card == null) return false;
            if (!Regex.IsMatch(card.Front, regex)) return false;
            if (!Regex.IsMatch(card.Reverse, regex)) return true;
            if (card.Description.Length != 0 && !Regex.IsMatch(card.Description, regex)) return true;
            return true;
        }

        private bool ValidateNewDeckDto(NewDeckDto newDeck)
        {
            string regex = "^[a-zA-Z0-9]([a-zA-Z0-9 ]){4,32}[a-zA-Z0-9]$";

            if (newDeck == null) return false;
            if (!Regex.IsMatch(newDeck.Title, regex)) return false;

            return true;
        }
    }
}
