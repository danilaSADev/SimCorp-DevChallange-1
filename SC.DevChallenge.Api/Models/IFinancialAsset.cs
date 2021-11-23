using System;

namespace SCDevChallengeApi
{
    public interface IFinancialAsset
    {
        string Portfolio { get; }
        string Owner { get; }
        string Instrument { get; }
        DateTime Datetime { get; }
        double Price { get; }
    }
}
