using C19K.Wpf.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C19K.Wpf.Service
{
    public class FatalCaseService : IC19Service
    {
        public string FilePath
        {
            get
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings["FatalPath"] ?? throw new Exception("Missing Configuration: File Path");
            }
        }
        public IReaderService Reader => IoC.Get<CsvReader>();
        public IEnumerable<CaseStatus> GetCummiliativeCases(IEnumerable<CaseStatus> statuses)
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

        public IEnumerable<CaseStatus> GetDailyCases(IEnumerable<CaseStatus> statuses)
        {
            return statuses;
        }
    }
}
