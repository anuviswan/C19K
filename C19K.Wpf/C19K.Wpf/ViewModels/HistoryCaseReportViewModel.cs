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
using System.Windows.Media;

namespace C19K.Wpf.ViewModels
{
    [ReportDescriptionAttribute(Description = "Total Cases In Kerala", Title = "Total Cases")]
    public class HistoryCaseReportViewModel:Screen,IReportViewModel<HistoryOfCasesService>
    {
        public List<GraphRecord> DistrictWiseCummilativeCases { get; set; }
        public List<GraphRecord> StateWideCummilativeCases { get; set; }
        public List<GraphRecord> StateWideDailyCases { get; set; }

        public List<GraphRecord> TopFiveInfectedDistricts { get; set; }
        public HistoryCaseReportViewModel()
        {
            DisplayName = "History";
        }

        protected override async void OnViewAttached(object view, object context)
        {
            await Reload();
        }
        public async Task Reload()
        {
            DistrictWiseCummilativeCases = await GetDistrictWiseCummilativeCasesAsync();
            StateWideCummilativeCases = await GetStateWideCummilativeCasesAsync();
            StateWideDailyCases = await GetStateWideDailyCasesAsync();
            TopFiveInfectedDistricts = await GetTopFiveInfectedDistrictsAsync();
        }

        private async Task<List<GraphRecord>> GetTopFiveInfectedDistrictsAsync()
        {
            var casesRecorded = await C19Service.GetCummilativeCases();
            return casesRecorded.Where(x => x.District != District.State)
                .GroupBy(x=>x.District)
                .Select(x=> new GraphRecord { Date = x.Max(c=>c.Date), Key = x.Key.ToString(), Value = x.OrderByDescending(c=>c.Date).First().Count})
                .OrderByDescending(x=>x.Value)
                .ToList();
        }

        private async Task<List<GraphRecord>> GetStateWideDailyCasesAsync()
        {
            var casesRecorded = await C19Service.GetDailyCases();
            return casesRecorded.Where(x => x.District == District.State).CastAsGraphRecord().ToList();
        }

        private async Task<List<GraphRecord>> GetDistrictWiseCummilativeCasesAsync()
        {
            var casesRecorded = await C19Service.GetCummilativeCases();
            return casesRecorded.Where(x=>x.District!= District.State).CastAsGraphRecord().ToList();
        }

        private async Task<List<GraphRecord>> GetStateWideCummilativeCasesAsync()
        {
            var casesRecorded = await C19Service.GetCummilativeCases();
            return casesRecorded.Where(x => x.District == District.State).CastAsGraphRecord().ToList();
        }

        public GenericC19Service<HistoryOfCasesService> C19Service { get; set; } = new GenericC19Service<HistoryOfCasesService>();
    }
}
