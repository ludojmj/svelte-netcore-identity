using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services.Interfaces;

namespace Server.Controllers;

[Route("api/[controller]")]
public class StuffController : ControllerBase
{
    // GET STUFF LIST api/stuff?page=2&search=xxx
    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] int page,
        [FromQuery] string search,
        [FromServices] IStuffService stuffService)
    {
        StuffModel result = string.IsNullOrWhiteSpace(search)
            ? await stuffService.GetListAsync(page)
            : await stuffService.SearchListAsync(search);
        return Ok(result);
    }

    // CREATE STUFF api/stuff + {body}
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] DatumModel input,
        [FromServices] IStuffService stuffService)
    {
        DatumModel result = await stuffService.CreateAsync(input);
        return CreatedAtAction(nameof(Read), new { id = result.Id }, result);
    }

    // READ STUFF api/stuff/id
    [HttpGet("{id}")]
    public async Task<IActionResult> Read(
        string id,
        [FromServices] IStuffService stuffService)
    {
        DatumModel result = await stuffService.ReadAsync(id);
        return Ok(result);
    }

    // UPDATE STUFF api/stuff/id + {body}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] DatumModel input,
        [FromServices] IStuffService stuffService)
    {
        DatumModel result = await stuffService.UpdateAsync(id, input);
        return Ok(result);
    }

    // DELETE STUFF api/stuff/id
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        string id,
        [FromServices] IStuffService stuffService)
    {
        await stuffService.DeleteAsync(id);
        return NoContent();
    }
}
