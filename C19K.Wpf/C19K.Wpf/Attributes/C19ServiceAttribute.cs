using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C19K.Wpf.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class ,AllowMultiple =false)]    
    public class C19ServiceAttribute:Attribute
    {
        public string Description { get; set; }
        public string Title { get; set; }

    }
}
