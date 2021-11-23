using System;
using System.Web;

namespace SCDevChallengeApi.BLL
{
    public class DateOperations
    {
        public static int TimeslotInterval { get; } = 10000;
        public static DateTime TimeslotBegin { get; } = new DateTime(2018, 1, 1);

        public int DateToTimeslot(DateTime input)
        {
            TimeSpan diff = input - TimeslotBegin;
            int inTimeslots = (int)diff.TotalSeconds / TimeslotInterval;
            return inTimeslots * TimeslotInterval;
        }

        public DateTime TimeslotToDate(int timeslot)
        {
            DateTime dateTime = TimeslotBegin.AddSeconds(timeslot);
            return dateTime;
        }
        public bool ParseDateTime(string str, out DateTime date)
        {
            str = HttpUtility.UrlDecode(str);
            return DateTime.TryParse(str, out date);
        }
    }
}
