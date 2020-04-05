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
        IEnumerable<CaseStatus> CummiliativeCases(IEnumerable<CaseStatus> statuses);
        IEnumerable<CaseStatus> DailyCases(IEnumerable<CaseStatus> statuses);
        string FilePath { get; }
    }
}
