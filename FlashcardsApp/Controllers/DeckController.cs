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
    /// Class <c>DeckController</c> filled with endpoints concerening deck management.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class DeckController : ControllerBase
    {
        private readonly string _connectionString;
        /// <summary>
        /// Model containing database queries concerning deck management.
        /// </summary>
        private readonly IDeckModel _deckModel;

        /// <summary>
        /// Constructor injecting dependency of: \
        /// - repository for commissioning database calls, \
        /// - configuration for acquiring connectionString which might come in handy.
        /// </summary>
        /// <param name="configuration">Configuration stored in appsettings.json</param>
        /// <param name="repo">Repository containing all database queries.</param>
        public DeckController(IConfiguration configuration, IFlashcardsRepository repo)
        {
            _connectionString = configuration.GetConnectionString("SQLServer")!;
            _deckModel = repo._deckModel;
            
        }

        /// <summary>
        /// Function <c>GetDeck</c> gets deck info by its id, if this particular authenticated user owns it.
        /// </summary>
        /// <param name="deckId">Id of deck stored in database.</param>
        /// <returns> Returns, if successful, Ok() with whole deck information including its cards. \
        /// Otherwise BadRequest(400) with adequate message. </returns>
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

            int deckIdInt;
            bool isIdInt = int.TryParse(deckId, out deckIdInt);
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

        /// <summary>
        /// Function <c>CreateDeck</c> creates deck with given title and description.
        /// </summary>
        /// <param name="deck">Its title and description.</param>
        /// <returns> Returns Ok() if successful, otherwise BadRequest(400).</returns>
        [HttpPost("create"), Authorize]
        public async Task<IActionResult> CreateDeck([FromBody] NewDeckDto deck)
        {
            // Validate deck data and user id
            bool isValid = ValidateNewDeckDto(deck);
            if (isValid is false)
                return BadRequest();

            var userIdString = Request.Cookies["userId"];
            try
            {
                UserIdToken.ParseTokenToInt(userIdString, out int userId);

                // Try creating deck in database
                bool isCorrect = await _deckModel.CreateDeck(deck, userId);
                if (isCorrect)
                    return Ok();
                else
                    return BadRequest();
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Function <c>GetPublicDecks</c> gets all public decks withing database. (have isPrivate set on 0)
        /// </summary>
        /// <returns>Returns list of public decks.</returns>
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

        /// <summary>
        /// Function <c>AddCard</c> adds card to a deck, if deck exists and user is allowed to.
        /// </summary>
        /// <param name="card">Card information, which is id of deck this card is added to, front, reverse and description.</param>
        /// <returns>Returns Ok() containing new card's Id when successful. /
        /// Otherwise BadRequest(400) with adequate message.</returns>
        [HttpPost("addcard"), Authorize]
        public async Task<ActionResult<string>> AddCard([FromBody] CardDto card)
        {
            // Validate card input
            bool result = ValidateCardDto(card);
            if (result is false)
                return BadRequest("Invalid card values");

            string json = "";
            try
            {
                // Validate userId
                var userIdString = Request.Cookies["userId"];
                UserIdToken.ParseTokenToInt(userIdString, out int userId);

                // Try adding card to deck in database
                string cardId = await _deckModel.AddCardToDeck(card, userId);
                if(Int32.Parse(cardId) >= 0)
                {
                    json = JsonSerializer.Serialize(cardId);
                    return Ok(json);
                }
                else
                {
                    return BadRequest("User does not own this deck");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Function <c>DeleteCard</c> deletes card from deck if deck exists and user is allowed to.
        /// </summary>
        /// <param name="id">Id of a to-be-deleted card.</param>
        /// <returns>Returns Ok() when successful, /
        /// BadRequest(400) otherwise.</returns>
        [HttpDelete("card"), Authorize]
        public async Task<IActionResult> DeleteCard([FromQuery] string id)
        {
            // Validate cardId
            int cardId;
            bool isIdInt = int.TryParse(id, out cardId);
            if (!isIdInt)
            {
                return BadRequest();
            }

            try
            {
                // Validate userId
                var userIdString = Request.Cookies["userId"];
                UserIdToken.ParseTokenToInt(userIdString, out int userId);

                // Try deleting card from deck in database
                bool isCorrect = await _deckModel.DeleteCardFromDeck(cardId, userId);
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

        /// <summary>
        /// Function <c>DeleteDeck</c> deletes deck in two particular ways: \
        /// - If user is creator of this deck, delete it globally. \
        /// - If user acquired it from public decks, delete it personally.
        /// </summary>
        /// <param name="id">Id of a to-be-deleted deck.</param>
        /// <returns>Returns Ok() when succssful.</returns>
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

            int deckId;
            bool isIdInt = int.TryParse(id, out deckId);
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

        /// <summary>
        /// Function <c>PublishDeck</c> makes deck public (available to use for everyone).
        /// </summary>
        /// <param name="id">Id of a deck to be made public.</param>
        /// <returns>Returns Ok() if deck was made public, \
        /// BadRequest() otherwise.</returns>
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
                // Validate userId
                var userIdString = Request.Cookies["userId"];
                UserIdToken.ParseTokenToInt(userIdString, out int userId);

                // Try changing deck's value isPublic from 0 to 1 in database
                var isCorrect = await _deckModel.PublishDeck(deckId, userId);
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

        /// <summary>
        /// Validation if card's contents are valid for database.
        /// </summary>
        /// <param name="card">Card containing front, reverse and description.</param>
        /// <returns>Returns True if validation is passed, otherwise false.</returns>
        private bool ValidateCardDto(CardDto card)
        {
            if (card == null) return false;
            if (card.Front.Length > 128 || card.Front.Length < 3) return false;
            if (card.Reverse.Length > 128 || card.Reverse.Length < 3) return true;
            if (card.Description.Length != 0 && card.Description.Length < 3 || card.Description.Length > 128) return false;
            return true;
        }

        /// <summary>
        /// Validation of deck's contents.
        /// </summary>
        /// <param name="newDeck">Deck containing title and description</param>
        /// <returns>Returns True if validation is passed, otherwise false.</returns>
        private bool ValidateNewDeckDto(NewDeckDto newDeck)
        {

            if (newDeck == null) return false;
            if (newDeck.Title.Length > 32 || newDeck.Title.Length < 6) return false;
            if (newDeck.Description.Length > 128) return false;

            return true;
        }
    }
}
