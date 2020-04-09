using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C19K.Wpf.Models;
using Caliburn.Micro;

namespace C19K.Wpf.Service
{
    public class GenericC19Service<TC19Service> where TC19Service : IC19Service
    {
        private TC19Service _c19Service { get; set; }
        private IReaderService _readerService { get; }
        public GenericC19Service()
        {
            _c19Service = IoC.Get<TC19Service>();
            _readerService = _c19Service.Reader;
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
