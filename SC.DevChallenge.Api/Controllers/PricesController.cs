using SC.DevChallenge.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Collections;
using System;

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
        public string Average(string portfolio, string owner, string instrument, string dateTime)
        {
            var result = _financialStorage.CalculateAvarage(portfolio, owner, instrument, dateTime);
            return result;
        }

        // TODO wire up with unit testing
    }
}
