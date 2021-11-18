using SC.DevChallenge.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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
        public ActionResult Average(string portfolio, string owner, string instrument, string dateTime)
        {
            var result = _financialStorage.CalculateAvarage(portfolio, owner, instrument, dateTime);
            if (result == "404")
            {
                return NotFound();
            }
            return Ok(result);
        }

        // TODO wire up with unit testing
    }
}
