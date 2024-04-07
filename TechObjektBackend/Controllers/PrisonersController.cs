using System;
using Microsoft.AspNetCore.Mvc;
using Plotly.NET;
using Plotly.NET.LayoutObjects;
using TechObjektBackend.Services;
using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning;
using Plotly.NET.ImageExport;

namespace TechObjektBackend.Controllers
{
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

        [HttpGet("plotly/png")]
        public IActionResult GetPlotlyChartAsPNG()
        {
            var data = _prisonersServices.GetAsync().Result;

            
            var groupedData = data.GroupBy(p => p.Year);

            var x = groupedData.Select(g => g.Key).ToArray();
            var y = groupedData.Select(g => g.Sum(p => p.Value)).ToArray();

            LinearAxis xAxis = new LinearAxis();
            xAxis.SetValue("title", "Year");
            xAxis.SetValue("showgrid", false);
            xAxis.SetValue("showline", true);

            LinearAxis yAxis = new LinearAxis();
            yAxis.SetValue("title", "Total Prisoners");
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
            trace.SetValue("name", "Total Prisoners");

            var chart = GenericChart
                        .ofTraceObject(false, trace)
                        .WithLayout(layout);

            var filePath = "D:\\TechObiektFrontend\\src\\images\\prisoners_chart.png"; 
            chart.SavePNG(filePath);

            return Ok();
        }

        [HttpGet("average-prisoners-per-country")]
        public IActionResult GetAveragePrisonersPerCountry()
        {
            var data = _prisonersServices.GetAsync().Result;

            var averagePrisonersPerCountry = data.GroupBy(p => p.Country)
                                                  .Select(g => new
                                                  {
                                                      Country = g.Key,
                                                      AveragePrisoners = g.Average(p => p.Value)
                                                  })
                                                  .ToList();

            return Ok(averagePrisonersPerCountry);
        }

        [HttpGet("prisoner-type-distribution")]
        public IActionResult GetPrisonerTypeDistribution()
        {
            var data = _prisonersServices.GetAsync().Result;

            var prisonerTypeDistribution = data.GroupBy(p => p.PrisionerType)
                                               .Select(g => new
                                               {
                                                   PrisonerType = g.Key,
                                                   TotalPrisoners = g.Sum(p => p.Value)
                                               })
                                               .ToList();

            return Ok(prisonerTypeDistribution);
        }

        [HttpGet("prisoners-in-countries-by-year")]
        public IActionResult GetPrisonersInCountriesByYear()
        {
            var data = _prisonersServices.GetAsync().Result;

            var prisonersInCountriesByYear = data.GroupBy(p => new { p.Country, p.Year })
                                                 .Select(g => new
                                                 {
                                                     Country = g.Key.Country,
                                                     Year = g.Key.Year,
                                                     TotalPrisoners = g.Sum(p => p.Value)
                                                 })
                                                 .ToList();

            return Ok(prisonersInCountriesByYear);
        }

        [HttpGet("trend-analysis")]
        public IActionResult GetTrendAnalysis()
        {
            var data = _prisonersServices.GetAsync().Result;

            var trendAnalysis = data.GroupBy(p => p.Year)
                                    .OrderBy(g => g.Key)
                                    .Select(g => new
                                    {
                                        Year = g.Key,
                                        TotalPrisoners = g.Sum(p => p.Value)
                                    })
                                    .ToList();

            return Ok(trendAnalysis);
        }

      

    }
}
