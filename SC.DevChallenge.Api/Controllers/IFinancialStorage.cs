using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SC.DevChallenge.Api.Models
{
    public interface IFinancialStorage
    {
         IActionResult CalculateAvarage(string? portfolio, string? owner, string? instrument, string dateTime);
    }
}