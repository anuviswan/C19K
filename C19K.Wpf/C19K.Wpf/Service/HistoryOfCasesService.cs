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

        public IEnumerable<Status> RetrieveInformation(IEnumerable<Status> statuses)
        {
            return statuses;
        }
    }
}
