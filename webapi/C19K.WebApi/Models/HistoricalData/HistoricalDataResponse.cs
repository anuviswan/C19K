using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace C19K.WebApi.Models.HistoricalData
{
    public class HistoricalDataResponse
    {
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public int TotalUnderObservation { get; set; }
        public int TotalUnderHomeQuarantine { get; set; }
        public int TotalSymptomaticHospitilized { get; set; }
        public int NumberOfPersonsHospitalizedToday { get; set; }
        public int TotalPositiveActiveCases { get; set; }
    }
}