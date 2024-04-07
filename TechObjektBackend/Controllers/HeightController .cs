using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Plotly.NET;
using Plotly.NET.ImageExport;
using Plotly.NET.LayoutObjects;
using System.Diagnostics.Metrics;
using TechObjektBackend.Models;
using TechObjektBackend.Services;

namespace TechObjektBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HeightController : ControllerBase
    {
        private readonly HeightDataService _heightDataService;
        private readonly ILogger<HeightController> _logger;

        public HeightController(HeightDataService heightDataService, ILogger<HeightController> logger)
        {
            _heightDataService = heightDataService;
            _logger = logger;
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

        [HttpGet("plotly/png")]
        public IActionResult GetPlotlyChartAsPNG()
        {
            var data = _heightDataService.GetAsync().Result;

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
            trace.SetValue("name", "Wysokość");

            var chart = GenericChart
                        .ofTraceObject(false, trace)
                        .WithLayout(layout);

            var filePath = "D:\\TechObiektFrontend\\src\\images\\height_chart.png";
            chart.SavePNG(filePath);

            return Ok();
        }
        [HttpGet("mean")]
        public async Task<ActionResult<double>> GetMeanHeight()
        {
            var meanHeight = await _heightDataService.CalculateMeanHeightAsync();
            return Ok(meanHeight);
        }

        [HttpGet("median")]
        public async Task<ActionResult<double>> GetMedianHeight()
        {
            var medianHeight = await _heightDataService.CalculateMedianHeightAsync();
            return Ok(medianHeight);
        }

        [HttpGet("standard-deviation")]
        public async Task<ActionResult<double>> GetStandardDeviation()
        {
            var standardDeviation = await _heightDataService.CalculateStandardDeviationAsync();
            return Ok(standardDeviation);
        }

        [HttpGet("tallest-country")]
        public async Task<IActionResult> GetTallestCountry()
        {
            var data = await _heightDataService.GetAsync(); 

            
            var groupedData = data.GroupBy(h => h.kraj);

            
            var averageHeights = groupedData.Select(group =>
                new
                {
                    Country = group.Key,
                    AverageHeight = group.Average(h => h.wartosc)
                });

            
            var tallestCountry = averageHeights.OrderByDescending(x => x.AverageHeight).FirstOrDefault();

            if (tallestCountry == null)
            {
                return NotFound("Brak danych dotyczących wzrostu.");
            }

            return Ok(new { TallestCountry = tallestCountry.Country, AverageHeight = tallestCountry.AverageHeight });
        }

    }
}
