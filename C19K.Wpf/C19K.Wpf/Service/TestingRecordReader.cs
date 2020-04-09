using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using C19K.Wpf.Models;
using LazyCache;

namespace C19K.Wpf.Service
{
    public class TestingRecordReader : IReaderService
    {
        private IAppCache cache = new CachingService();
        public async Task<IEnumerable<CaseStatus>> Read(string filePath)
        {
            var cacheKey = $"{nameof(TestingRecordReader)}-{filePath}";
            return await cache.GetOrAddAsync<IEnumerable<CaseStatus>>(cacheKey, () => ReadInternal(filePath));
        }
        private Task<IEnumerable<CaseStatus>> ReadInternal(string filePath)
        {
            var result = new List<CaseStatus>();
            CultureInfo provider = CultureInfo.InvariantCulture;

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var dataRead = csv.GetRecords<dynamic>().ToList();
                foreach (var item in dataRead)
                {
                    if (string.IsNullOrWhiteSpace(item.Date))
                        continue;

                    var valueDictionary = new RouteValueDictionary(item);

                    result.AddRange(Enum.GetNames(typeof(District)).Select(x => new CaseStatus
                    {
                        District = (District)Enum.Parse(typeof(District), x),
                        Date = System.DateTime.ParseExact(item.Date, "dd-MM-yyyy", provider),
                        Count = int.TryParse((string)valueDictionary["TotalSamplesTested"], out var value) ? value : 0
                    }));
                }
                return Task.FromResult(result.AsEnumerable());
            }
        }
    }
}
