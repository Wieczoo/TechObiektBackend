using System;
using Microsoft.AspNetCore.Mvc;
using TechObjektBackend.Services;
namespace TechObjektBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PrisonersController : ControllerBase
{
    private readonly PrisonersServices _prisonersServices;

    public PrisonersController(PrisonersServices PrisonersServices) =>
        _prisonersServices = PrisonersServices;

    [HttpGet]
    public async Task<List<Prisoners>> Get() =>
        await _prisonersServices.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Prisoners>> Get(string id)
    {
        var Prisoners = await _prisonersServices.GetAsync(id);

        if (Prisoners is null)
        {
            return NotFound();
        }

        return Prisoners;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Prisoners newPrisoners)
    {
        await _prisonersServices.CreateAsync(newPrisoners);

        return CreatedAtAction(nameof(Get), new { id = newPrisoners.Id }, newPrisoners);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Prisoners updatedPrisoners)
    {
        var Prisoners = await _prisonersServices.GetAsync(id);

        if (Prisoners is null)
        {
            return NotFound();
        }

        updatedPrisoners.Id = Prisoners.Id;

        await _prisonersServices.UpdateAsync(id, updatedPrisoners);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var Prisoners = await _prisonersServices.GetAsync(id);

        if (Prisoners is null)
        {
            return NotFound();
        }

        await _prisonersServices.RemoveAsync(id);

        return NoContent();
    }
}

