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
        public GenericC19Service()
        {
            _c19Service = IoC.Get<T>();
        }
        public IEnumerable<Status> Get()
        {
            var fileReader = IoC.Get<IReaderService>();
            var data = fileReader.Read(_c19Service.FilePath);
            return _c19Service.RetrieveInformation(data);
        }
    }
}
