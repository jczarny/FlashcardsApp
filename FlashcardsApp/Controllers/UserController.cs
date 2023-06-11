using FlashcardsApp.Models;
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
    [Route("[controller]")]
    [EnableCors("_myAllowSpecificOrigins")]
    public class UserController : ControllerBase
    {
        private readonly FlashcardsContext _context;
        public UserController(FlashcardsContext context) {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<User> Get()
        {
            //var query = _context.Cards.Where(s => s.DeckId == 1);

            return Enumerable.Range(1, 5).Select(index => new User
            {
                Id = 0,
                Username = "temporary user",
                Password = "pwd"
            })
            .ToArray();
        }

        [HttpPost("login")]
        public IActionResult Post(string? username, string? password)
        {
            var l = _context.Users
                .Where(o => o.Username == username && o.Password == password).ToList();
            if (l.Count() == 1)
                return Ok(1);
            else
                return BadRequest(0);
        }
    }
}
