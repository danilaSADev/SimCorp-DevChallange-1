
using System.Runtime.Serialization;
using System;

namespace SCDevChallengeApi.Models
{
    [Serializable]
    public class AvarageGetResult : ISerializable
    {
        public DateTime Date { get; }
        public double Price { get; }

        public AvarageGetResult(DateTime date, double price)
        {
            Date = date;
            Price = price;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("date", Date);
            info.AddValue("price", Price);
        }
    }
}
