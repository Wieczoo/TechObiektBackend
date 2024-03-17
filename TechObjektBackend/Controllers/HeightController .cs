using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechObjektBackend.Models;
using TechObjektBackend.Services;
using TechObjektBackend.Models;

namespace TechObjektBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HeightController : ControllerBase
    {
        private readonly HeightDataService _heightDataService;

        public HeightController(HeightDataService heightDataService)
        {
            _heightDataService = heightDataService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Height>>> Get()
        {
            var data = await _heightDataService.GetAsync();
            return Ok(data);
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Height>> Get(string id)
        {
            var data = await _heightDataService.GetAsync(id);
            if (data == null)
                return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult<Height>> Post(Height data)
        {
            await _heightDataService.CreateAsync(data);
            return CreatedAtAction(nameof(Get), new { id = data.Id }, data);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Put(string id, Height data)
        {
            var existingData = await _heightDataService.GetAsync(id);
            if (existingData == null)
                return NotFound();

            data.Id = existingData.Id;
            await _heightDataService.UpdateAsync(id, data);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingData = await _heightDataService.GetAsync(id);
            if (existingData == null)
                return NotFound();

            await _heightDataService.RemoveAsync(id);
            return NoContent();
        }

        
    }
}
