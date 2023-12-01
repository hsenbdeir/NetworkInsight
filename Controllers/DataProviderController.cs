using Microsoft.AspNetCore.Mvc;
using NetworkInsight.Services;

namespace NetworkInsight.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataProviderController : ControllerBase
    {

        private AggregatorService aggregatorService;

        public DataProviderController( AggregatorService aggregatorService)
        {
            this.aggregatorService = aggregatorService;
        }
        [HttpGet]
        public IActionResult GetKpiData(string globalFilterValues, string dateTimeFilterValue)
        {
            var result = aggregatorService.GetKpiData(globalFilterValues, dateTimeFilterValue);


            if (result == null)
            {
                return BadRequest("Invalid groupBy parameter");
            }

            return Ok(result);
        }

        [HttpGet("Aggregate")]
        public IActionResult Aggregate()
        {
            Console.WriteLine("Inside the Aggregator controller");
            Console.WriteLine("---------------------------------------------------------------");
            aggregatorService.Aggregate();
            Console.WriteLine("Aggregated");
            return Ok();
        }
    }


}
