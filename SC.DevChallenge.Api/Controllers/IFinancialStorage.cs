using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SC.DevChallenge.Api.Models
{
    public interface IFinancialStorage
    {
        /// <summary>
        /// Calculates avarage price depending dateTime interval and other parameters.
        /// </summary>
        /// <param name="portfolio"> Declares a specific portfolio.</param>
        /// <param name="owner"> Declares an owner of financial assets.</param>
        /// <param name="instrument"> Declares an instrument of asset.</param>
        /// <param name="dateTime">Declares an interval on which asset was bought.</param>
        /// <returns> 
        /// On Success returns <see cref="ActionResult"/> 
        /// with JSON string which includes avarage price and 
        /// timeslot interval start. 
        /// Otherwise returns negative <see cref="ActionResult"/>. 
        /// </returns>
        ActionResult CalculateAvarage(string portfolio, string owner, string instrument, string dateTime);
        ActionResult CalculateAvarageBenchmarked(string portifolio, string dateTime);
        ActionResult CalculateAvarageAggregated(string portfolion, string startDate, string endDate, string intervals);
    }
}