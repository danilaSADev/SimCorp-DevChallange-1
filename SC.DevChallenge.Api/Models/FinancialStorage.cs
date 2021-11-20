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
        private int _timeslotInterval = 10000;

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
            DateTime realDateTime;

            dateTime = HttpUtility.UrlDecode(dateTime);

            if (!DateTime.TryParse(dateTime, out realDateTime))
            {
                return new BadRequestResult();
            }

            int startTimeSlot = DateToTimeslot(realDateTime);
            int endTimeSlot = startTimeSlot + _timeslotInterval;
            var query = from asset in AssetsList
                        where asset.Portfolio == portfolio && 
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
            DateTime realDateTime;

            dateTime = HttpUtility.UrlDecode(dateTime);

            if (!DateTime.TryParse(dateTime, out realDateTime))
            {
                return new BadRequestResult();
            }

            int startTimeSlot = DateToTimeslot(realDateTime);
            int endTimeSlot = startTimeSlot + _timeslotInterval;

            var query = from asset in AssetsList
                        where asset.Portfolio == portifolio &&
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
            // TODO parse start and end dates

            // TODO 

            // TODO replace with real return result
            throw new NotImplementedException();
        }
    }
}
