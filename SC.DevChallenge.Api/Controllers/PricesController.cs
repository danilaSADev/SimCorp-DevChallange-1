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
        [HttpGet("average")]
        public string Average(string portfolio, string owner, string instrument, string dateTime)
        {
            FinancialStorage storage = new FinancialStorage();
            var result = storage.CalculateAvarage(portfolio, owner, instrument, dateTime);
            return result;
        }

        [HttpGet("test")]
        public string Test() 
        {
            FinancialStorage storage = new FinancialStorage();
            StringBuilder result = new StringBuilder();
            result.Append(Average("","","","") + "\n");
            result.Append(Average("123", "123", "12", "1231") + "\n");
            return $"Loaded assets = {storage.AssetsList.Count}";
        }
    }
}
