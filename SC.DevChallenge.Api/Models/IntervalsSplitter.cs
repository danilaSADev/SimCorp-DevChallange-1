using System;
using System.Linq;
using System.Collections.Generic;

namespace SCDevChallengeApi.Models
{
    public class IntervalsSplitter
    {

        private int _startTimeslot;
        private int _endTimeslot;
        private int _intervals;

        public IntervalsSplitter(int starttimeslot, int endtimeslot, int intervals)
        {
            _startTimeslot = starttimeslot;
            _endTimeslot = endtimeslot;
            _intervals = intervals;
        }

        /// <summary>
        /// Calculate groups for intervals with given parameters in the constructor.
        /// </summary>
        /// <returns>Queue <see cref="IEnumerable{int}"/> of intervals for each groups.</returns>
        public IEnumerable<int> CalculateIntervals()
        {

            var intervalsCollection = new Queue<int>();

            int timeslotsCount = (_endTimeslot - _startTimeslot) / FinancialStorage._timeslotInterval;

            double avarageTimeslotsCount = timeslotsCount / _intervals;

            intervalsCollection.Enqueue(timeslotsCount);

            for (int i = 0; i < _intervals; i++)
            {
                var element = intervalsCollection.Dequeue();
                double value = element / 2.0;
                intervalsCollection.Enqueue((int)Math.Floor(value));
                intervalsCollection.Enqueue((int)Math.Ceiling(value));
                intervalsCollection.OrderBy(value => value);
            }

            if (intervalsCollection.Count() > _intervals)
            {
                var firstElement = intervalsCollection.Dequeue();
                double avarage = firstElement / _intervals;
                while (firstElement > 0)
                {
                    int ceiledAvarage = (int)Math.Ceiling(avarage);
                    int flooredAvarage = (int)Math.Floor(avarage);
                    if (firstElement >= ceiledAvarage)
                    {
                        firstElement -= ceiledAvarage;
                        intervalsCollection.Enqueue(intervalsCollection.Dequeue() + ceiledAvarage);
                    } 
                    else if (firstElement >= flooredAvarage)
                    {
                        firstElement -= flooredAvarage;
                        intervalsCollection.Enqueue(intervalsCollection.Dequeue() + ceiledAvarage);
                    } 
                    else
                    {
                        intervalsCollection.Enqueue(intervalsCollection.Dequeue() + firstElement);
                    }
                }
            }

            return intervalsCollection;
        }

    }
}
