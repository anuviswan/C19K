using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C19K.Wpf.Attributes;
using C19K.Wpf.Models;

namespace C19K.Wpf.Service
{
    public class HistoryOfCasesService : IC19Service
    {
        public string FilePath
        {
            get
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings["HistoryPath"] ?? throw new Exception("Missing Configuration: File Path");
            }
        }

        public IEnumerable<CaseStatus> CummiliativeCases(IEnumerable<CaseStatus> statuses)
        {
            foreach (var groupedDistrict in statuses.GroupBy(x => x.District))
            {
                var currentCount = 0;
                foreach (var current in groupedDistrict.ToList().OrderBy(x => x.Date))
                {
                    currentCount += current.Count;
                    yield return new CaseStatus
                    {
                        District = groupedDistrict.Key,
                        Date = current.Date,
                        Count = currentCount,
                    };
                }
            }
        }

        public IEnumerable<CaseStatus> DailyCases(IEnumerable<CaseStatus> statuses)
        {
            return statuses;
        }

       
    }
}
