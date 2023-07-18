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
            // Validate userId and deckId
            var userIdString = Request.Cookies["userId"];
            int userId;
            try
            {
                UserIdToken.ParseTokenToInt(userIdString, out userId);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            bool isIdInt = int.TryParse(deckId, out int deckIdInt);
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
            // Validate deck data and user id
            bool isValid = ValidateNewDeckDto(deck);
            if (isValid is false)
                return BadRequest();

            var userIdString = Request.Cookies["userId"];
            int userId;
            try
            {
                UserIdToken.ParseTokenToInt(userIdString, out userId);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            try
            {
                // Try creating deck in database
                bool isCorrect = await _deckModel.CreateDeck(deck, userId);
                if(isCorrect)
                    return Ok();
                else
                    return BadRequest();
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
                // Validate userId
                var userIdString = Request.Cookies["userId"];
                UserIdToken.ParseTokenToInt(userIdString, out int userId);

                // Get public decks from database
                List<DeckDto> decks = await _deckModel.GetPublicDecks(userId);
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
        public async Task<ActionResult<int>> AddCard([FromBody] CardDto card)
        {
            // Validate card input
            bool result = ValidateCardDto(card);
            if (result is false)
                return BadRequest("Invalid card values");

            string json = "";
            try
            {
                // Try adding card to deck in database
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
        public async Task<IActionResult> DeleteCard([FromQuery] string id)
        {
            // Validate cardId
            bool isIdInt = int.TryParse(id, out int cardId);
            if (!isIdInt)
            {
                return BadRequest();
            }

            try
            {
                // Try deleting card from deck in database
                bool isCorrect = await _deckModel.DeleteCardFromDeck(cardId);
                if (isCorrect)
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
        public async Task<IActionResult> DeleteDeck([FromQuery] string id)
        {
            // Validate userId and deckId
            var userIdString = Request.Cookies["userId"];
            int userId;
            try
            {
                UserIdToken.ParseTokenToInt(userIdString, out userId);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            

            bool isIdInt = int.TryParse(id, out int deckId);
            if (!isIdInt)
            {
                return BadRequest();
            }

            try
            {
                // Try deleting deck from database
                var isCorrect = await _deckModel.DeleteDeck(userId, deckId);

                if(isCorrect) 
                    return Ok();
                else 
                    return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // Make deck public (available to use for everyone)
        [HttpPatch("publish"), Authorize]
        public async Task<IActionResult> PublishDeck([FromQuery] string id)
        {
            // Validate deckId 
            bool isIdInt = int.TryParse(id, out int deckId);
            if (!isIdInt)
            {
                return BadRequest();
            }

            try
            {
                // Try changing deck's value isPublic from 0 to 1 in database
                var isCorrect = await _deckModel.PublishDeck(deckId);
                if (isCorrect) 
                    return Ok();
                else 
                    return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        private bool ValidateCardDto(CardDto card)
        {
            if (card == null) return false;
            if (card.Front.Length > 128 || card.Front.Length < 3) return false;
            if (card.Reverse.Length > 128 || card.Reverse.Length < 3) return true;
            if (card.Description.Length != 0 && card.Description.Length < 3 || card.Description.Length > 128) return false;
            return true;
        }

        private bool ValidateNewDeckDto(NewDeckDto newDeck)
        {

            if (newDeck == null) return false;
            if (newDeck.Title.Length > 32 || newDeck.Title.Length < 6) return false;

            return true;
        }
    }
}
