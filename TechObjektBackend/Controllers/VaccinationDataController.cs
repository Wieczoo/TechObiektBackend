using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechObjektBackend.Services;
using Plotly.NET;
using Plotly.NET.ImageExport;
using Plotly.NET.LayoutObjects;
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

    [HttpGet("plotly/png")]
    public IActionResult GetPlotlyChartAsPNG()
    {
        var data = _vaccinationDataService.GetAsync().Result;

        var x = data.Select(h => h.rok).ToArray();
        var y = data.Select(h => h.wartosc).ToArray();

        LinearAxis xAxis = new LinearAxis();
        xAxis.SetValue("title", "Rok");
        xAxis.SetValue("showgrid", false);
        xAxis.SetValue("showline", true);

        LinearAxis yAxis = new LinearAxis();
        yAxis.SetValue("title", "Wartość");
        yAxis.SetValue("showgrid", false);
        yAxis.SetValue("showline", true);

        Layout layout = new Layout();
        layout.SetValue("xaxis", xAxis);
        layout.SetValue("yaxis", yAxis);
        layout.SetValue("showlegend", true);

        Trace trace = new Trace("scatter");
        trace.SetValue("x", x);
        trace.SetValue("y", y);
        trace.SetValue("mode", "markers");
        trace.SetValue("name", "Wartość");

        var chart = GenericChart
                    .ofTraceObject(false, trace)
                    .WithLayout(layout);

        var filePath = "D:\\TechObiektFrontend\\src\\images\\vaccination_chart.png";
        chart.SavePNG(filePath);

        return Ok();
    }

    [HttpGet("analysis/simple1")]
    public IActionResult SimpleAnalysis1()
    {
        var data = _vaccinationDataService.GetAsync().Result;

        
        var result = data
            .GroupBy(v => v.kraj)
            .Select(group => new { Country = group.Key, TotalVaccinations = group.Sum(v => v.wartosc) })
            .OrderByDescending(x => x.TotalVaccinations)
            .ToList();

        return Ok(result);
    }

    [HttpGet("analysis/simple2")]
    public IActionResult SimpleAnalysis2()
    {
        var data = _vaccinationDataService.GetAsync().Result;

        
        var result = data
            .GroupBy(v => v.rok)
            .Select(group => new { Year = group.Key, AverageVaccinations = group.Average(v => v.wartosc) })
            .ToList();

        return Ok(result);
    }

    [HttpGet("analysis/simple3")]
    public IActionResult SimpleAnalysis3()
    {
        var data = _vaccinationDataService.GetAsync().Result;

        
        var result = data
            .GroupBy(v => v.rodzaj_choroby)
            .Select(group => new { Disease = group.Key, Count = group.Count() })
            .OrderByDescending(x => x.Count)
            .ToList();

        return Ok(result);
    }

    [HttpGet("analysis/complex")]
    public IActionResult ComplexAnalysis()
    {
        var data = _vaccinationDataService.GetAsync().Result;

        
        var result = data
            .GroupBy(v => v.czas_typ_szczepienia)
            .Select(group => new { TimeType = group.Key, TotalVaccinations = group.Sum(v => v.wartosc) })
            .OrderByDescending(x => x.TotalVaccinations)
            .ToList();

        return Ok(result);
    }
}

