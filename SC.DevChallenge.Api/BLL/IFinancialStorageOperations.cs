using Microsoft.AspNetCore.Mvc;

namespace SCDevChallengeApi
{
    public interface IFinancialStorageOperations
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
        /// <summary>
        /// Calculates avarage benchmarked price dateTime interval and other parameters.
        /// </summary>
        /// <param name="portifolio"> Declares portfolio to include in calculations.</param>
        /// <param name="dateTime"> Declares an interval on which asset was bought.</param>
        /// <returns> 
        /// On Success returns <see cref="ActionResult"/> 
        /// with JSON string which includes avarage price and 
        /// timeslot interval start. 
        /// Otherwise returns negative <see cref="ActionResult"/>. 
        /// </returns>
        ActionResult CalculateAvarageBenchmarked(string portifolio, string dateTime);
        /// <summary>
        /// Calculates avarage benchmarked price (depending on portfolio) for several intervals independently.
        /// </summary>
        /// <param name="portfolio"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="intervals"></param>
        /// <returns> 
        /// On Success returns <see cref="ActionResult"/> 
        /// with JSON string which includes avarage price and 
        /// timeslot interval start. 
        /// Otherwise returns negative <see cref="ActionResult"/>. 
        /// </returns>
        ActionResult CalculateAvarageAggregated(string portfolio, string startDate, string endDate, string intervals);
    }
}
