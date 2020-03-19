using C19K.WebApi.Models.Historical;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace C19K.WebApi.Controllers
{
    public class KeralaController
    {
        public List<HistoricalResponse> GetHistoricalData(HistoricalRequest historialcalRequest)
        {
            return Enumerable.Range(1,10).Select(x=> new HistoricalResponse { Location = x.ToString(), Count = x }).ToList();
        }
    }
}