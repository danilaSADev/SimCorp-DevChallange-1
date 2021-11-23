using System;
using SCDevChallengeApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace SCDevChallengeApi.Controllers
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
            //var result = _financialStorage.CalculateAvarage(portfolio, owner, instrument, date);
            //return result;
            throw new NotImplementedException();    
        }

        [HttpGet("benchmark")]
        public ActionResult Benchmark(string portfolio, string date)
        {
            //var result = _financialStorage.CalculateAvarageBenchmarked(portfolio, date);
            throw new NotImplementedException();
            //return result;
        }

        [HttpGet("aggregate")]
        public ActionResult Aggregate(string portfolio, string startdate, string enddate, string intervals)
        {
            //var result = _financialStorage.CalculateAvarageAggregated(portfolio, startdate, enddate, intervals);
            throw new NotImplementedException();
            //return result;
        }

    }
}
