using FlashcardsApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
    }
}
