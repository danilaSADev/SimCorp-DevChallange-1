using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace SC.DevChallenge.Api.Models
{
    public class AvaragePriceBenchmark
    {
        private string CalculateAvarageBenchmarked(IEnumerable<FinancialAsset> query, int startTimeslot)
        {
            QuantileCalculations quantiles = new QuantileCalculations(query);
            var benchmarkedPrices = quantiles.GetBenchmarkedAssets();

            if (query.Count() <= 0)
            {
                return "not found";
            }

            double avarage = query.Average(asset => asset.Price);
            DateTime date = FinancialStorage.TimeslotToDate(startTimeslot);

            AvarageGetResult result = new AvarageGetResult(date, avarage);
            string jsonString = JsonSerializer.Serialize(result);

            return jsonString;
        }

        public ActionResult GetAvarageBenchmarked(IEnumerable<FinancialAsset> query, int startTimeslot)
        {
            var result = CalculateAvarageBenchmarked(query, startTimeslot);

            if (result == "not found")
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(result);
        }

        public ActionResult GetAvarageAggregatedBenchmarked(IEnumerable<FinancialAsset> query, int startTimeslot, int endTimeslot, int intervalsValue)
        {

            IntervalsSplitter intervalsSplitter = new IntervalsSplitter(startTimeslot, endTimeslot, intervalsValue);

            var intervalsCollection = intervalsSplitter.CalculateIntervals();

            int firstInterval = startTimeslot + FinancialStorage._timeslotInterval;
            int lastInterval = startTimeslot;

            List<AvarageGetResult> groupedAssets = new List<AvarageGetResult>();

            foreach (var group in intervalsCollection)
            {
                lastInterval = firstInterval + group * FinancialStorage._timeslotInterval;

                var intervalsGroup = from asset in query
                                     where FinancialStorage.DateToTimeslot(asset.Datetime) < lastInterval &&
                                           FinancialStorage.DateToTimeslot(asset.Datetime) >= firstInterval
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
