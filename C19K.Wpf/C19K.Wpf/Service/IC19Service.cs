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
        IEnumerable<Status> RetrieveInformation(IEnumerable<Status> statuses);
        string FilePath { get; }
    }
}
