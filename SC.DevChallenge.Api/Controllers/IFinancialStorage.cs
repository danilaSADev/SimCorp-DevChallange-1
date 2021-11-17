namespace SC.DevChallenge.Api.Models
{
    public interface IFinancialStorage
    {
        string CalculateAvarage(string? portfolio, string? owner, string? instrument, string dateTime);
    }
}