using System;

namespace SC.DevChallenge.Api.Models
{
    public class FromCSVAssetParser
    {
        private string _dataToParse;
        public FromCSVAssetParser(string data)
        {
            _dataToParse = data;
        }

        public FinancialAsset Parse()
        {
            string[] data = _dataToParse.Split(",");
            if (data.Length == 0)
            {
                return new FinancialAsset();
            }
            string portfolio = data[0];
            string owner = data[1];
            string instrument = data[2];
            DateTime date = DateTime.Parse(data[3]);
            data[4] = data[4].Replace(".", ",");
            double price = Double.Parse(data[4]);
            FinancialAsset result = new FinancialAsset(portfolio, owner, instrument, date, price);
            return result;
        }

    }
}
