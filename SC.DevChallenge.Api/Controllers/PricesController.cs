using SC.DevChallenge.Api.Models;
using Microsoft.AspNetCore.Mvc;
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




            return $"Loaded assets = {storage.AssetsList.Count}";


        }
    }
}
