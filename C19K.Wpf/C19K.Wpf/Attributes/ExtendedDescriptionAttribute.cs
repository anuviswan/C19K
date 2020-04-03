using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C19K.Wpf.Attributes
{
    public class ExtendedDescriptionAttribute:Attribute
    {
        public string Description { get; set; }
        public OxyColor DefaultColor { get; set; }

    }
}
