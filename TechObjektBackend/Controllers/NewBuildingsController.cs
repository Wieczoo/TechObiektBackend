using System;
using Microsoft.AspNetCore.Mvc;
using TechObjektBackend.Services;
namespace TechObjektBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewBuildingsController : ControllerBase
{
    private readonly NewBuildingsServices _NewBuildingsServices;

    public NewBuildingsController(NewBuildingsServices NewBuildingsServices) =>
        _NewBuildingsServices = NewBuildingsServices;

    [HttpGet]
    public async Task<List<NewBuildings>> Get() =>
        await _NewBuildingsServices.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<NewBuildings>> Get(string id)
    {
        var NewBuildings = await _NewBuildingsServices.GetAsync(id);

        if (NewBuildings is null)
        {
            return NotFound();
        }

        return NewBuildings;
    }

    [HttpPost]
    public async Task<IActionResult> Post(NewBuildings newBuildings)
    {
        await _NewBuildingsServices.CreateAsync(newBuildings);

        return CreatedAtAction(nameof(Get), new { id = newBuildings.Id }, newBuildings);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, NewBuildings updatedNewBuildings)
    {
        var NewBuildings = await _NewBuildingsServices.GetAsync(id);

        if (NewBuildings is null)
        {
            return NotFound();
        }

        updatedNewBuildings.Id = NewBuildings.Id;

        await _NewBuildingsServices.UpdateAsync(id, updatedNewBuildings);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var NewBuildings = await _NewBuildingsServices.GetAsync(id);

        if (NewBuildings is null)
        {
            return NotFound();
        }

        await _NewBuildingsServices.RemoveAsync(id);

        return NoContent();
    }

    [HttpGet("predictions/{id:length(24)}")]
    public async Task<IActionResult> Predictions(string id)
    {
        var NewBuildings = await _NewBuildingsServices.PredictFutureData(id);

        if (NewBuildings is null)
        {
            return NotFound();
        }

        return (IActionResult)NewBuildings;
    }
}

