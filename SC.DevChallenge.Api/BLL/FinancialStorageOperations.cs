using System;
using Microsoft.AspNetCore.Mvc;
using SCDevChallengeApi.Models;
using System.Text.Json;
using System.Linq;

namespace SCDevChallengeApi.BLL
{
    public class FinancialStorageOperations
    {
        private IFinancialStorage _financialStorage;
        private AvaragePriceBenchmark _avaragePriceBenchmark = new AvaragePriceBenchmark();
        private DateOperations DateOperations { get; } = new DateOperations();
        #nullable enable
        public FinancialOperationResult CalculateAvarage(string? portfolio, string owner, string instrument, string dateTime)
        {
            if (!DateOperations.ParseDateTime(dateTime, out DateTime realDateTime))
            {
                return new FinancialOperationResult(new BadRequestResult());
            }

            int startTimeSlot = DateOperations.DateToTimeslot(realDateTime);
            int endTimeSlot = startTimeSlot + DateOperations.TimeslotInterval;
            // TODO access somehow valeus from FinancialStorage
            var query = from asset in _financialStorage.AssetsList where
                        (portfolio != null ? asset.Portfolio.ToLower() == portfolio.ToLower() : null) |
                        asset.Instrument == instrument |
                        asset.Owner == owner |
                        (DateOperations.DateToTimeslot(asset.Datetime) < endTimeSlot &&
                        DateOperations.DateToTimeslot(asset.Datetime) >= startTimeSlot)
                        select asset;

            if (query.Count() <= 0)
            {
                return new FinancialOperationResult(new NotFoundResult());
            }

            double avarage = query.Average(asset => asset.Price);
            DateTime date = DateOperations.TimeslotToDate(startTimeSlot);

            AvarageGetResult result = new AvarageGetResult(date, avarage);
            string jsonString = JsonSerializer.Serialize(result);

            return new FinancialOperationResult(new OkObjectResult(jsonString));
        }
        #nullable disable
        public ActionResult CalculateAvarageBenchmarked(string portfolio, string dateTime)
        {
            if (!DateOperations.ParseDateTime(dateTime, out DateTime realDateTime))
            {
                return new BadRequestResult();
            }

            int startTimeSlot = DateOperations.DateToTimeslot(realDateTime);
            int endTimeSlot = startTimeSlot + DateOperations.TimeslotInterval;

            var query = from asset in _financialStorage.AssetsList
                        where asset.Portfolio.ToLower() == portfolio.ToLower() &&
                        DateOperations.DateToTimeslot(asset.Datetime) < endTimeSlot &&
                        DateOperations.DateToTimeslot(asset.Datetime) >= startTimeSlot
                        select asset;

            return _avaragePriceBenchmark.GetAvarageBenchmarked(query, startTimeSlot);
        }

        public ActionResult CalculateAvarageAggregated(string portfolio, string startDate, string endDate, string intervals)
        {
            if (!(DateOperations.ParseDateTime(startDate, out DateTime startDateTime) &&
                DateOperations.ParseDateTime(endDate, out DateTime endDateTime) &&
                int.TryParse(intervals, out int intervalsValue)))
            {
                return new BadRequestResult();
            }

            int startTimeslot = DateOperations.DateToTimeslot(startDateTime);
            int endTimeslot = DateOperations.DateToTimeslot(endDateTime);

            var portfoliosInDateRange = from asset in _financialStorage.AssetsList
                                        where asset.Portfolio.ToLower() == portfolio.ToLower() &&
                                        DateOperations.DateToTimeslot(asset.Datetime) < endTimeslot &&
                                        DateOperations.DateToTimeslot(asset.Datetime) >= startTimeslot
                                        select asset;

            return _avaragePriceBenchmark.GetAvarageAggregatedBenchmarked(portfoliosInDateRange, startTimeslot, endTimeslot, intervalsValue);
        }
    }
}
