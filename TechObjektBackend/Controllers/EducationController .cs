using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechObjektBackend.Services;

namespace TechObjektBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EducationController : ControllerBase
    {
        private readonly EducationDataService _educationDataService;

        public EducationController(EducationDataService educationDataService)
        {
            _educationDataService = educationDataService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Education>>> Get()
        {
            var data = await _educationDataService.GetAsync();
            return Ok(data);
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Education>> Get(string id)
        {
            var data = await _educationDataService.GetAsync(id);
            if (data == null)
                return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult<Education>> Post(Education data)
        {
            await _educationDataService.CreateAsync(data);
            return CreatedAtAction(nameof(Get), new { id = data.Id }, data);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Put(string id, Education data)
        {
            var existingData = await _educationDataService.GetAsync(id);
            if (existingData == null)
                return NotFound();

            data.Id = existingData.Id;
            await _educationDataService.UpdateAsync(id, data);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingData = await _educationDataService.GetAsync(id);
            if (existingData == null)
                return NotFound();

            await _educationDataService.RemoveAsync(id);
            return NoContent();
        }
    }
}
