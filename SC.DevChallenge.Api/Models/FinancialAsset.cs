using System;

namespace SC.DevChallenge.Api.Models
{
    public class FinancialAsset
    {
        public string Portfolio { get; }
        public string Owner { get; }
        public string Instrument { get; }
        public DateTime Datetime { get; }
        public double Price { get; }

        public FinancialAsset(string portfolio, string owner, string instrument, DateTime datetime, double price)
        {
            Portfolio = portfolio;
            Owner = owner;
            Instrument = instrument;
            Datetime = datetime;
            Price = price;
        }

        public FinancialAsset() : this("", "", "", new DateTime(2018, 1, 1), 0)
        {   }
    }
}
