using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using SCDevChallengeApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace SCDevChallengeApi.BLL
{
    public class AvaragePriceBenchmark
    {
        private AvarageGetResult CalculateAvarageBenchmarked(IEnumerable<IFinancialAsset> query, int startTimeslot)
        {
            DateOperations dateOperations = new DateOperations();
            QuantileCalculations quantiles = new QuantileCalculations(query);
            var benchmarkedPrices = quantiles.GetBenchmarkedAssets();

            if (query.Count() <= 0)
            {
                return new EmptyAvarageGetResult();
            }

            double avarage = query.Average(asset => asset.Price);
            DateTime date = dateOperations.TimeslotToDate(startTimeslot);

            AvarageGetResult result = new AvarageGetResult(date, avarage);
            return result;
        }

        public ActionResult GetAvarageBenchmarked(IEnumerable<IFinancialAsset> query, int startTimeslot)
        {
            var result = CalculateAvarageBenchmarked(query, startTimeslot);
            return result;
        }

        public ActionResult GetAvarageAggregatedBenchmarked(IEnumerable<IFinancialAsset> query, int startTimeslot, int endTimeslot, int intervalsValue)
        {

            IntervalsSplitter intervalsSplitter = new IntervalsSplitter(startTimeslot, endTimeslot, intervalsValue);

            var intervalsCollection = intervalsSplitter.CalculateIntervals();

            int firstInterval = startTimeslot + DateOperations.TimeslotInterval;
            int lastInterval = startTimeslot;

            List<AvarageGetResult> groupedAssets = new List<AvarageGetResult>();

            foreach (var group in intervalsCollection)
            {
                lastInterval = firstInterval + group * DateOperations.TimeslotInterval;

                var intervalsGroup = from asset in query
                                     where DateOperations.DateToTimeslot(asset.Datetime) < lastInterval &&
                                           DateOperations.DateToTimeslot(asset.Datetime) >= firstInterval
                                     select asset;

                QuantileCalculations quantiles = new QuantileCalculations(intervalsGroup);
                var benchmarkedPrices = quantiles.GetBenchmarkedAssets();

                if (intervalsGroup.Count() <= 0)
                {
                    return new NotFoundResult();
                }

                double avarage = intervalsGroup.Average(asset => asset.Price);
                DateTime date = FinancialStorage.TimeslotToDate(firstInterval);

                AvarageGetResult result = new AvarageGetResult(date, avarage);
                groupedAssets.Add(result);

                firstInterval = lastInterval;
            }

            string jsonString = JsonSerializer.Serialize(groupedAssets);

            return new OkObjectResult(jsonString);
        }
    }
}
