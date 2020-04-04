using C19K.Wpf.Models;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace C19K.Wpf.Service
{
    public class CsvReader:IReaderService
    {
       public IEnumerable<Status> Read(string filePath)
        {
            var result = new List<Status>();
            CultureInfo provider = CultureInfo.InvariantCulture;

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var dataRead = csv.GetRecords<dynamic>().ToList();
                foreach(var item in dataRead)
                {
                    if (string.IsNullOrWhiteSpace(item.Date))
                        continue;

                    var valueDictionary = new RouteValueDictionary(item);
                    
                    result.AddRange(Enum.GetNames(typeof(District)).Select(x => new Status
                    {
                        District = (District)Enum.Parse(typeof(District), x),
                        Date = System.DateTime.ParseExact(item.Date, "dd-MM-yyyy", provider),
                        Count = int.TryParse((string)valueDictionary[x], out var value) ? value : 0
                    })); 
                }
                return result;
            }
        }
    }

}
