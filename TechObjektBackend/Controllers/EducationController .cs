using Microsoft.AspNetCore.Mvc;
using Plotly.NET.LayoutObjects;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plotly.NET;
using Plotly.NET.LayoutObjects;
using Plotly.NET.ImageExport;
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

        [HttpGet("plotly/png")]
        public IActionResult GetEducationChartAsPNG()
        {
            var data = _educationDataService.GetAsync().Result;

            
            var groupedData = data.GroupBy(e => e.rok_szkolny);

            var x = groupedData.Select(g => g.Key).ToArray();
            var y = groupedData.Select(g => g.Sum(e => e.wartosc)).ToArray();

            LinearAxis xAxis = new LinearAxis();
            xAxis.SetValue("title", "Rok szkolny");
            xAxis.SetValue("showgrid", false);
            xAxis.SetValue("showline", true);

            LinearAxis yAxis = new LinearAxis();
            yAxis.SetValue("title", "Suma wartości");
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
            trace.SetValue("name", "Suma wartości");

            var chart = GenericChart
                        .ofTraceObject(false, trace)
                        .WithLayout(layout);

            var filePath = "D:\\TechObiektFrontend\\src\\images\\education_chart.png"; 
            chart.SavePNG(filePath);

            return Ok();
        }

            [HttpGet("education-trend-analysis")]
            public IActionResult EducationTrendAnalysis()
            {
                var data = _educationDataService.GetAsync().Result;

                
                var trendAnalysis = data.GroupBy(e => e.rok_szkolny)
                                        .OrderBy(g => g.Key)
                                        .Select(g => new
                                        {
                                            Year = g.Key,
                                            TotalValue = g.Sum(e => e.wartosc)
                                        })
                                        .ToList();

                

                return Ok(trendAnalysis);
            }
            [HttpGet("education-distribution-by-school-type")]
            public IActionResult EducationDistributionBySchoolType()
            {
                var data = _educationDataService.GetAsync().Result;

                
                var distributionBySchoolType = data.GroupBy(e => e.typ_szkoly)
                                                   .Select(g => new
                                                   {
                                                       SchoolType = g.Key,
                                                       TotalValue = g.Sum(e => e.wartosc)
                                                   })
                                                   .ToList();

                return Ok(distributionBySchoolType);
            }
            [HttpGet("education-change-by-gender")]
            public IActionResult EducationChangeByGender()
            {
                var data = _educationDataService.GetAsync().Result;

                
                var changeByGender = data.GroupBy(e => e.plec_absolwenta)
                                          .Select(g => new
                                          {
                                              Gender = g.Key,
                                              TotalValue = g.Sum(e => e.wartosc)
                                          })
                                          .ToList();

                return Ok(changeByGender);
            }
            [HttpGet("education-correlation-analysis")]
            public IActionResult EducationCorrelationAnalysis()
            {
                var data = _educationDataService.GetAsync().Result;

                

                return Ok("Placeholder for correlation analysis");
            }

    }
}
