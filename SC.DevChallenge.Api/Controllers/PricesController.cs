using System;
using SC.DevChallenge.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace SC.DevChallenge.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PricesController : ControllerBase
    {
        private IFinancialStorage _financialStorage;
        public PricesController(IFinancialStorage storage)
        {
            _financialStorage = storage;
        }

        [HttpGet("average")]
        public ActionResult Average(string portfolio, string owner, string instrument, string date)
        {
            var result = _financialStorage.CalculateAvarage(portfolio, owner, instrument, date);
            return result as ActionResult;
        }

        [HttpGet("benchmark")]
        public ActionResult Benchmark(string portfolio, string date)
        {
            var result = _financialStorage.CalculateAvarageBenchmarked(portfolio, date);
            return result as ActionResult;
        }

        [HttpGet("aggregate")]
        public ActionResult Aggregate(string portfolio, string startdate, string enddate, string intervals)
        {
            return Ok();
        }

        // TODO wire up with unit testing
    }
}
