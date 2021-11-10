using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Repository.Interfaces;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    public class StuffController : ControllerBase
    {
        private readonly IStuffRepo _stuffRepo;

        public StuffController(IStuffRepo stuffRepo)
        {
            _stuffRepo = stuffRepo;
        }

        // GET STUFF LIST api/stuff?page=2&search=xxx
        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] int page, [FromQuery] string search)
        {
            StuffModel result = string.IsNullOrWhiteSpace(search)
                ? await _stuffRepo.GetListAsync(page)
                : await _stuffRepo.SearchListAsync(search);
            return Ok(result);
        }

        // CREATE STUFF api/stuff + {body}
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DatumModel input)
        {
            DatumModel result = await _stuffRepo.CreateAsync(input);
            return CreatedAtAction(nameof(Read), new { id = result.Id }, result);
        }

        // READ STUFF api/stuff/id
        [HttpGet("{id}")]
        public async Task<IActionResult> Read(string id)
        {
            DatumModel result = await _stuffRepo.ReadAsync(id);
            return Ok(result);
        }

        // UPDATE STUFF api/stuff/id + {body}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] DatumModel input)
        {
            DatumModel result = await _stuffRepo.UpdateAsync(id, input);
            return Ok(result);
        }

        // DELETE STUFF api/stuff/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _stuffRepo.DeleteAsync(id);
            return NoContent();
        }
    }
}
