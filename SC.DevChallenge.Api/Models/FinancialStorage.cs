using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Linq;

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
        // TODO configure date-to-slot and slot-to-date so they use 2018.01.01 as a start date
        // TODO configure slot-to-date so they would return exactly the start point of the specified timeslot
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

        public string CalculateAvarage(string portfolio, string owner, string instrument, string dateTime)
        {
            DateTime realDateTime = DateTime.Parse(dateTime);
            int startTimeSlot = DateToTimeslot(realDateTime);
            int endTimeSlot = startTimeSlot + _timeslotInterval;
            var query = from asset in AssetsList
                        where asset.Portfolio == portfolio && 
                        asset.Instrument == instrument && 
                        asset.Owner == owner && 
                        DateToTimeslot(asset.Datetime) <= endTimeSlot && 
                        DateToTimeslot(asset.Datetime) >= startTimeSlot
                        select asset;

            if (query.Count() <= 0)
            {
                return "404";
            }

            double avarage = query.Average(asset => asset.Price);
            DateTime date = TimeslotToDate(endTimeSlot);

            AvarageGetResult result = new AvarageGetResult(date, avarage);
            string jsonString = JsonSerializer.Serialize(result);

            return jsonString;
        }

    }
}
