using Xunit;
using Microsoft.AspNetCore.Mvc;
using SCDevChallengeApi.Models;

namespace FinancialStorageTests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("Fannie Mae", "15/03/2018 17:34:50")]
        public void Test1(string portfolio, string date)
        {
            FinancialStorage storage = new FinancialStorage();

            Assert.Equal(new OkObjectResult("{ date: 15/03/2018 17:26:40,price: 133.71}"), storage.CalculateAvarageBenchmarked(portfolio, date));
            //Assert.Equal(new OkObjectResult("{ date: 15/03/2018 17:26:40,price: 133.71}"), storage.CalculateAvarageAggregated(portfolio, date));

        }
    }
}