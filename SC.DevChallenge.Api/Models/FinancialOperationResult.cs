using Microsoft.AspNetCore.Mvc;

namespace SCDevChallengeApi.Models
{
    public class FinancialOperationResult
    {
        public ActionResult GetResult { get; }
        public FinancialOperationResult(ActionResult result)
        {
            GetResult = result;
        }
    }
}
