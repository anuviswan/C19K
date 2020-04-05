using C19K.Wpf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C19K.Wpf.Service
{
    public interface IC19Service
    {
        IEnumerable<CaseStatus> GetCummiliativeCases(IEnumerable<CaseStatus> statuses);
        IEnumerable<CaseStatus> GetDailyCases(IEnumerable<CaseStatus> statuses);
        string FilePath { get; }
    }
}
