using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Service.Interfaces;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        // GET USER LIST api/user?page=2&search=xxx
        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] int page,
            [FromQuery] string search,
            [FromServices] IUserService userService)
        {
            DirectoryModel result = string.IsNullOrWhiteSpace(search)
                ? await userService.GetListAsync(page)
                : await userService.SearchListAsync(search);
            return Ok(result);
        }

        // CREATE USER api/user + {body}
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] UserModel input,
            [FromServices] IUserService userService)
        {
            UserModel result = await userService.CreateAsync(input);
            return CreatedAtAction(nameof(Read), new { id = result.Id }, result);
        }

        // READ USER api/user/id
        [HttpGet("{id}")]
        public async Task<IActionResult> Read(
            string id,
            [FromServices] IUserService userService)
        {
            UserModel result = await userService.ReadAsync(id);
            return Ok(result);
        }

        // UPDATE USER api/user/id + {body}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            string id,
            [FromBody] UserModel input,
            [FromServices] IUserService userService)
        {
            UserModel result = await userService.UpdateAsync(id, input);
            return Ok(result);
        }

        // DELETE USER api/user/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            string id,
            [FromServices] IUserService userService)
        {
            await userService.DeleteAsync(id);
            return NoContent();
        }
    }
}
