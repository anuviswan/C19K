using C19K.Wpf.Attributes;
using C19K.Wpf.ExtensionMethods;
using C19K.Wpf.Models;
using C19K.Wpf.Service;
using Caliburn.Micro;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace C19K.Wpf.ViewModels
{
    [ReportDescriptionAttribute(Description = "Active Cases In Kerala", Title = "Active Cases")]
    public class ActiveCaseReportViewModel:Screen,IReportViewModel<ActiveCaseService>
    {
        public ActiveCaseReportViewModel()
        {
            DisplayName = "Active Cases";
        }

        protected override async void OnViewAttached(object view, object context)
        {
            await Reload();
        }
        public async Task Reload()
        {
            DistrictWiseActiveCases = await GetDistrictWiseActiveCases();
            StateWideActiveCases = await GetStateWideActiveCases();
        }

        private async Task<List<GraphRecord>> GetDistrictWiseActiveCases()
        {
            var casesRecorded = await C19Service.GetCummilativeCases();
            return casesRecorded.Where(x => x.District != District.State).CastAsGraphRecord().ToList();
        }

        private async Task<List<GraphRecord>> GetStateWideActiveCases()
        {
            var casesRecorded = await C19Service.GetCummilativeCases();
            return casesRecorded.Where(x => x.District == District.State).CastAsGraphRecord().ToList();
        }
               
        public List<GraphRecord> DistrictWiseActiveCases { get; set; }
        public List<GraphRecord> StateWideActiveCases { get; set; }
        public GenericC19Service<ActiveCaseService> C19Service { get; set; } = new GenericC19Service<ActiveCaseService>();
    }

   
}
