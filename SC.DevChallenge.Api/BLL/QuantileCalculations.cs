using System;
using System.Linq;
using System.Collections.Generic;

namespace SCDevChallengeApi.BLL
{
    public class QuantileCalculations
    {
        private IEnumerable<IFinancialAsset> _assetsCollection;
        public QuantileCalculations(IEnumerable<IFinancialAsset> collection)
        {
            _assetsCollection = collection;
        }
        private int CalculateThirdQuantile()
        {
            return (int)Math.Ceiling((3 * _assetsCollection.Count() - 3.0) / 4.0);
        }

        private int CalculateFirstQuantile()
        {
            return (int)Math.Ceiling((_assetsCollection.Count() - 1.0) / 4.0);
        }

        public IEnumerable<IFinancialAsset> GetBenchmarkedAssets()
        {
            var sortedAssets = _assetsCollection.OrderBy(asset => asset.Price);

            int quartile1 = CalculateFirstQuantile();
            int quartile3 = CalculateThirdQuantile();

            double avaragePrice = sortedAssets.Average(asset => asset.Price);

            double interQuartile = avaragePrice * quartile3 - avaragePrice * quartile1;
            double lowerBorder = avaragePrice * quartile1 - 1.5 * interQuartile;
            double upperBorder = avaragePrice * quartile3 + 1.5 * interQuartile;

            var result = from asset in sortedAssets
                         where asset.Price >= lowerBorder && asset.Price <= upperBorder
                         select asset;

            return result; 
        }

    }
}
