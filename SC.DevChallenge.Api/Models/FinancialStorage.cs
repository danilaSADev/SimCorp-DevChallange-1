using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace SC.DevChallenge.Api.Models
{
    public class FinancialStorage : IFinancialStorage
    {
        private readonly string CSV_PATH = Path.Combine(Environment.CurrentDirectory, @"Input\", "data.csv");
        public static readonly int _timeslotInterval = 10000;

        private DateTime _timeslotStart = new DateTime(2018, 1, 1);

        public List<FinancialAsset> AssetsList { get; } = new List<FinancialAsset>();
        public FinancialStorage() 
        {
            LoadFinancialInformation();
        }

        private int DateToTimeslot(DateTime input) 
        {
            TimeSpan diff = input - _timeslotStart;
            int inTimeslots = (int)diff.TotalSeconds / _timeslotInterval;
            return inTimeslots * _timeslotInterval;
        }

        private DateTime TimeslotToDate(int timeslot)
        {
            DateTime dateTime = _timeslotStart.AddSeconds(timeslot);
            return dateTime;
        }
        
        private bool ParseDateTime(string str, out DateTime date)
        {
            str = HttpUtility.UrlDecode(str);
            return DateTime.TryParse(str, out date);
        }

        private void LoadFinancialInformation() 
        {
            using (var reader = new StreamReader(CSV_PATH))
            {
                string line = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                    FromCSVAssetParser parser = new FromCSVAssetParser(line);
                    AssetsList.Add(parser.Parse());
                }
            }
        }

        public ActionResult CalculateAvarage(string portfolio, string owner, string instrument, string dateTime)
        {
            if (!ParseDateTime(dateTime, out DateTime realDateTime))
            {
                return new BadRequestResult();
            }

            int startTimeSlot = DateToTimeslot(realDateTime);
            int endTimeSlot = startTimeSlot + _timeslotInterval;
            var query = from asset in AssetsList
                        where asset.Portfolio.ToLower() == portfolio.ToLower() && 
                        asset.Instrument == instrument && 
                        asset.Owner == owner && 
                        DateToTimeslot(asset.Datetime) < endTimeSlot && 
                        DateToTimeslot(asset.Datetime) >= startTimeSlot
                        select asset;

            if (query.Count() <= 0)
            {
                return new NotFoundResult();
            }

            double avarage = query.Average(asset => asset.Price);
            DateTime date = TimeslotToDate(startTimeSlot);

            AvarageGetResult result = new AvarageGetResult(date, avarage);
            string jsonString = JsonSerializer.Serialize(result);

            return new OkObjectResult(jsonString);
        }

        public ActionResult CalculateAvarageBenchmarked(string portifolio, string dateTime)
        {
            if (!ParseDateTime(dateTime, out DateTime realDateTime))
            {
                return new BadRequestResult();
            }

            int startTimeSlot = DateToTimeslot(realDateTime);
            int endTimeSlot = startTimeSlot + _timeslotInterval;

            var query = from asset in AssetsList
                        where asset.Portfolio.ToLower() == portifolio.ToLower() &&
                        DateToTimeslot(asset.Datetime) < endTimeSlot &&
                        DateToTimeslot(asset.Datetime) >= startTimeSlot
                        select asset;

            QuantileCalculations quantiles = new QuantileCalculations(query);
            var benchmarkedPrices = quantiles.GetBenchmarkedAssets();

            if (query.Count() <= 0)
            {
                return new NotFoundResult();
            }

            double avarage = query.Average(asset => asset.Price);
            DateTime date = TimeslotToDate(startTimeSlot);

            AvarageGetResult result = new AvarageGetResult(date, avarage);
            string jsonString = JsonSerializer.Serialize(result);

            return new OkObjectResult(jsonString);
        }

        public ActionResult CalculateAvarageAggregated(string portfolio, string startDate, string endDate, string intervals)
        {
            if (!(ParseDateTime(startDate, out DateTime startDateTime) && 
                ParseDateTime(endDate, out DateTime endDateTime) && 
                int.TryParse(intervals, out int intervalsValue)))
            {
                return new BadRequestResult();
            }

            int startTimeslot = DateToTimeslot(startDateTime);
            int endTimeslot = DateToTimeslot(endDateTime);

            // Separate this logic from Financial Storage 

            IntervalsSplitter intervalsSplitter = new IntervalsSplitter(startTimeslot, endTimeslot, intervalsValue);

            var intervalsCollection = intervalsSplitter.CalculateIntervals();

            int firstInterval = startTimeslot;
            int lastInterval = startTimeslot;

            var portfoliosInDateRange = from asset in AssetsList
                                        where asset.Portfolio.ToLower() == portfolio.ToLower() &&
                                        DateToTimeslot(asset.Datetime) < endTimeslot &&
                                        DateToTimeslot(asset.Datetime) >= startTimeslot
                                        select asset;

            List<AvarageGetResult> groupedAssets = new List<AvarageGetResult>();

            foreach (var group in intervalsCollection)
            {
                lastInterval = firstInterval + group * _timeslotInterval;

                var intervalsGroup = from asset in portfoliosInDateRange
                                     where DateToTimeslot(asset.Datetime) < lastInterval &&
                                           DateToTimeslot(asset.Datetime) >= firstInterval
                                     select asset;

                QuantileCalculations quantiles = new QuantileCalculations(intervalsGroup);
                var benchmarkedPrices = quantiles.GetBenchmarkedAssets();

                if (intervalsGroup.Count() <= 0)
                {
                    return new NotFoundResult();
                }

                double avarage = intervalsGroup.Average(asset => asset.Price);
                DateTime date = TimeslotToDate(firstInterval);

                AvarageGetResult result = new AvarageGetResult(date, avarage);
                groupedAssets.Add(result);

                firstInterval = lastInterval;
            }

            string jsonString = JsonSerializer.Serialize(groupedAssets);

            return new OkObjectResult(jsonString);
        }
    }
}
