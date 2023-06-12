using FlashcardsApp.Dtos;
using FlashcardsApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace FlashcardsApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class UserController : ControllerBase
    {
        private readonly FlashcardsContext _context;
        public UserController(FlashcardsContext context) {
            _context = context;
        }

        [HttpGet, Authorize]
        public IEnumerable<User> Get()
        {
            //var query = _context.Cards.Where(s => s.DeckId == 1);

            return Enumerable.Range(1, 5).Select(index => new User
            {
                Id = 0,
                Username = "temporary user",
                PasswordHash = new byte[256],
                PasswordSalt = new byte[256]
            })
            .ToArray();
        }

        [HttpPost("login"), Authorize]
        public IActionResult Post(UserDto user)
        {
            //var l = _context.Users
            //    .Where(o => o.Username == user.Username && o.PasswordHash == user.Password).ToList();
            //if (l.Count() == 1)
            //    return Ok(1);
            //else
            //    return BadRequest(0);
            return Ok(1);
        }
    }
}
