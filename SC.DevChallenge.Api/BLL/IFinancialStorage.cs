using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SCDevChallengeApi.Models;

namespace SCDevChallengeApi
{
    public interface IFinancialStorage
    {
        /// <summary>
        /// List of all financial assets loaded from .csv file.
        /// </summary>
        List<FinancialAsset> AssetsList { get; }
       
    }
}