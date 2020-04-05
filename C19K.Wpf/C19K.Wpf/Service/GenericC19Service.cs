using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C19K.Wpf.Models;
using Caliburn.Micro;

namespace C19K.Wpf.Service
{
    public class GenericC19Service<T> where T : IC19Service
    {
        private T _c19Service { get; set; }
        private IReaderService _readerService { get; }
        public GenericC19Service()
        {
            _c19Service = IoC.Get<T>();
            _readerService = IoC.Get<IReaderService>();
        }
        public async Task<IEnumerable<CaseStatus>> GetCummilativeCases()
        {
            var data = await _readerService.Read(_c19Service.FilePath);
            return _c19Service.GetCummiliativeCases(data);
        }

        public async Task<IEnumerable<CaseStatus>> GetDailyCases()
        {
            var data = await _readerService.Read(_c19Service.FilePath);
            return _c19Service.GetDailyCases(data);
        }
    }
}
