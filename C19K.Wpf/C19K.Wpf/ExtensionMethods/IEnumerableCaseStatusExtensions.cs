using C19K.Wpf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C19K.Wpf.ExtensionMethods
{
    public static class IEnumerableCaseStatusExtensions
    {
        public static IEnumerable<GraphRecord> CastAsGraphRecord(this IEnumerable<CaseStatus> source)
        {
            return source.Select(x => new GraphRecord { Date = x.Date, Key = x.District.ToString(), Value = x.Count });
        }
    }
}
