using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Web;
using Microsoft.Data.Sqlite;

namespace SCDevChallengeApi.Models
{
    public class FinancialStorage : IFinancialStorage
    {
        private readonly string CSV_PATH = Path.Combine(Environment.CurrentDirectory, @"Input\", "data.csv");
        public static readonly int _timeslotInterval = 10000;
        private static readonly DateTime _timeslotStart = new DateTime(2018, 1, 1);

        //private SqliteConnection;

        private AvaragePriceBenchmark _avaragePriceBenchmark;
        
        public List<FinancialAsset> AssetsList { get; } = new List<FinancialAsset>();
        public FinancialStorage() 
        {
            _avaragePriceBenchmark = new AvaragePriceBenchmark();
            LoadFinancialInformation();
            LoadDataBase();
        }

        private void LoadDataBase()
        {
            using (var connection = new SqliteConnection($"DataSource={Environment.CurrentDirectory}/usersdata.db;Mode=ReadWriteCreate;"))
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = "CREATE TABLE FinancialStorage(_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, portfolio TEXT NOT NULL, owner TEXT NOT NULL, instrument TEXT NOT NULL)";
                command.ExecuteNonQuery();
            }
        }

        public static int DateToTimeslot(DateTime input) 
        {
            TimeSpan diff = input - _timeslotStart;
            int inTimeslots = (int)diff.TotalSeconds / _timeslotInterval;
            return inTimeslots * _timeslotInterval;
        }

        public static DateTime TimeslotToDate(int timeslot)
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

        public ActionResult CalculateAvarageBenchmarked(string portfolio, string dateTime)
        {
            if (!ParseDateTime(dateTime, out DateTime realDateTime))
            {
                return new BadRequestResult();
            }

            int startTimeSlot = DateToTimeslot(realDateTime);
            int endTimeSlot = startTimeSlot + _timeslotInterval;

            var query = from asset in AssetsList
                        where asset.Portfolio.ToLower() == portfolio.ToLower() &&
                        DateToTimeslot(asset.Datetime) < endTimeSlot &&
                        DateToTimeslot(asset.Datetime) >= startTimeSlot
                        select asset;

            return _avaragePriceBenchmark.GetAvarageBenchmarked(query, startTimeSlot);
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

            var portfoliosInDateRange = from asset in AssetsList
                                        where asset.Portfolio.ToLower() == portfolio.ToLower() &&
                                        FinancialStorage.DateToTimeslot(asset.Datetime) < endTimeslot &&
                                        FinancialStorage.DateToTimeslot(asset.Datetime) >= startTimeslot
                                        select asset;

            return _avaragePriceBenchmark.GetAvarageAggregatedBenchmarked(portfoliosInDateRange, startTimeslot, endTimeslot, intervalsValue);
        }
    }
}
