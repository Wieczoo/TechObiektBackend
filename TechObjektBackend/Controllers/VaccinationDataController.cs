using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechObjektBackend.Services;
namespace TechObjektBackend.Controllers;


[ApiController]
    [Route("api/[controller]")]
    public class VaccinationDataController : ControllerBase
    {
        private readonly VaccinationDataService _vaccinationDataService;

        public VaccinationDataController(VaccinationDataService vaccinationDataService)
        {
            _vaccinationDataService = vaccinationDataService;
        }

        [HttpGet]
        public async Task<ActionResult<List<VaccinationData>>> Get()
        {
            var data = await _vaccinationDataService.GetAsync();
            return Ok(data);
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<VaccinationData>> Get(string id)
        {
            var data = await _vaccinationDataService.GetAsync(id);
            if (data == null)
                return NotFound();
            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult<VaccinationData>> Post(VaccinationData data)
        {
            await _vaccinationDataService.CreateAsync(data);
            return CreatedAtAction(nameof(Get), new { id = data.Id }, data);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Put(string id, VaccinationData data)
        {
            var existingData = await _vaccinationDataService.GetAsync(id);
            if (existingData == null)
                return NotFound();

            data.Id = existingData.Id;
            await _vaccinationDataService.UpdateAsync(id, data);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingData = await _vaccinationDataService.GetAsync(id);
            if (existingData == null)
                return NotFound();

            await _vaccinationDataService.RemoveAsync(id);
            return NoContent();
        }
    }

