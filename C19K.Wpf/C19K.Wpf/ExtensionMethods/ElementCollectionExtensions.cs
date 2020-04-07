using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C19K.Wpf.ExtensionMethods
{
    public static class ElementCollectionExtensions
    {
        public static ElementCollection<T> AddRange<T>(this ElementCollection<T> source,IEnumerable<T> collectionToAdd) where T : Element
        {
            foreach(var item in collectionToAdd)
            {
                source.Add(item);
            }
            return source;
        }

    }
}
