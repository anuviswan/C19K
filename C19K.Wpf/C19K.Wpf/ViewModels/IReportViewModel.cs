using C19K.Wpf.Models;
using C19K.Wpf.Service;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C19K.Wpf.ViewModels
{
    public interface IReportViewModel:IScreen
    {
        Task Reload();
    }
    public interface IReportViewModel<TService>:IReportViewModel where TService : IC19Service,new()
    {
        GenericC19Service<TService> C19Service { get; set; }
    }
}
