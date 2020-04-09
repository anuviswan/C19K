using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C19K.Wpf.Models;
using Caliburn.Micro;

namespace C19K.Wpf.Service
{
    public class TestingDetailsService : IC19Service
    {
        public string FilePath
        {
            get
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings["TestingDetailsPath"] ?? throw new Exception("Missing Configuration: File Path");
            }
        }

        public IReaderService Reader => IoC.Get<TestingRecordReader>();

        public IEnumerable<CaseStatus> GetCummiliativeCases(IEnumerable<CaseStatus> statuses)
        {
            return statuses.Where(x=>x.District == District.State);
        }

        public IEnumerable<CaseStatus> GetDailyCases(IEnumerable<CaseStatus> statuses)
        {
            throw new NotImplementedException();
        }
    }
}
