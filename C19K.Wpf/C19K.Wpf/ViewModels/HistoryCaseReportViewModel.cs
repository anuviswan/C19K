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
    public class HistoryCaseReportViewModel:Screen, IReportViewModel
    {
        public List<GraphRecord> DistrictWiseCummilativeCases { get; set; }
        public List<GraphRecord> StateWideCummilativeCases { get; set; }
        public List<GraphRecord> ConfirmedCasesPerDay { get; set; }
        public List<GraphRecord> DistrictWiseDistributionOfConfirmedCases { get; set; }

        public List<GraphRecord> TotalTestsDonePerDay { get; set; }
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
            ConfirmedCasesPerDay = await GetStateWideDailyCasesAsync();
            DistrictWiseDistributionOfConfirmedCases = await GetDistrictWiseDistribution();
            TotalTestsDonePerDay = await GetTotalCasesDonePerDayAsync();
        }

        private async Task<List<GraphRecord>> GetTotalCasesDonePerDayAsync()
        {
            var casesRecorded = (await TestCasesService.GetCummilativeCases()).ToList();
            var casesPerDay = new[] { casesRecorded[0] }.Concat(casesRecorded.Zip(casesRecorded.Skip(1), (first, second) => new CaseStatus { Count = second.Count - first.Count, Date = second.Date, District = second.District }));
            return casesPerDay.CastAsGraphRecord().ToList();
        }

        private async Task<List<GraphRecord>> GetDistrictWiseDistribution()
        {
            var casesRecorded = await HistoryOfCasesService.GetCummilativeCases();
            return casesRecorded.Where(x => x.District != District.State)
                .GroupBy(x=>x.District)
                .Select(x=> new GraphRecord { Date = x.Max(c=>c.Date), Key = x.Key.ToString(), Value = x.OrderByDescending(c=>c.Date).First().Count})
                .ToList();
        }

        private async Task<List<GraphRecord>> GetStateWideDailyCasesAsync()
        {
            var casesRecorded = await HistoryOfCasesService.GetDailyCases();
            return casesRecorded.Where(x => x.District == District.State).CastAsGraphRecord().ToList();
        }

        private async Task<List<GraphRecord>> GetDistrictWiseCummilativeCasesAsync()
        {
            var casesRecorded = await HistoryOfCasesService.GetCummilativeCases();
            return casesRecorded.Where(x=>x.District!= District.State).CastAsGraphRecord().ToList();
        }

        private async Task<List<GraphRecord>> GetStateWideCummilativeCasesAsync()
        {
            var casesRecorded = await HistoryOfCasesService.GetCummilativeCases();
            return casesRecorded.Where(x => x.District == District.State).CastAsGraphRecord().ToList();
        }

        public GenericC19Service<HistoryOfCasesService> HistoryOfCasesService { get; set; } = new GenericC19Service<HistoryOfCasesService>();
        public GenericC19Service<TestingDetailsService> TestCasesService { get; set; } = new GenericC19Service<TestingDetailsService>();
    }
}
