using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Repository.Interfaces;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;

        public UserController(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        // GET USER LIST api/user?page=2&search=xxx
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] int page, [FromQuery] string search)
        {
            DirectoryModel result = string.IsNullOrWhiteSpace(search)
                ? await _userRepo.GetListAsync(page)
                : await _userRepo.SearchListAsync(search);
            return Ok(result);
        }

        // CREATE USER api/user + {body}
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserModel input)
        {
            UserModel result = await _userRepo.CreateAsync(input);
            return CreatedAtAction(nameof(Read), new { id = result.Id }, result);
        }

        // READ USER api/user/id
        [HttpGet("{id}")]
        public async Task<IActionResult> Read(string id)
        {
            UserModel result = await _userRepo.ReadAsync(id);
            return Ok(result);
        }

        // UPDATE USER api/user/id + {body}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserModel input)
        {
            UserModel result = await _userRepo.UpdateAsync(id, input);
            return Ok(result);
        }

        // DELETE USER api/user/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _userRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
