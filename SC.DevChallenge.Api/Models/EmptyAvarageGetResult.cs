using System;
using System.Runtime.Serialization;
using System.Text.Json;

namespace SCDevChallengeApi.Models
{
    public class EmptyAvarageGetResult : AvarageGetResult
    {
        public EmptyAvarageGetResult() : base(DateTime.Now, 0)
        {   }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("not found", "no value was found");
        }
    }
}
