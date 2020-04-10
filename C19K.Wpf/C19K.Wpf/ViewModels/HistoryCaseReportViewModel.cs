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
        public List<GraphRecord> TestStats { get; set; }
        public List<GraphRecord> OverviewStats { get; set; }
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
            TestStats = await GetTestStatsAsync();
            OverviewStats = await GetOverStatsAsync();
        }

        private async Task<List<GraphRecord>> GetOverStatsAsync()
        {
            var activeCasesCummilative = await ActiveCaseService.GetCummilativeCases();
            var historicalCasesCummilative = await HistoryOfCasesService.GetCummilativeCases();
            var fatalCaseCummilative = await FatalCaseService.GetCummilativeCases();

            var result = new List<GraphRecord>();
            var lastAvailableDate = historicalCasesCummilative.Where(x => x.District == District.State).OrderBy(x => x.Date).Last().Date;
            var totalConfirmedCases = historicalCasesCummilative.Where(x => x.District == District.State).OrderBy(x => x.Date).Last().Count;
            var totalActiveCases = activeCasesCummilative.Where(x => x.District == District.State).OrderBy(x => x.Date).Last().Count;
            var totalFatalCases = fatalCaseCummilative.Where(x=>x.District == District.State).Max(x=>x.Count);

            result.Add(new GraphRecord
            {
                Date = lastAvailableDate,
                Key = "Total Confirmed Cases",
                Value = totalConfirmedCases
            });

            result.Add(new GraphRecord
            {
                Date = lastAvailableDate,
                Key = "Total Active Cases",
                Value = totalActiveCases
            });

            result.Add(new GraphRecord
            {
                Date = lastAvailableDate,
                Key = "Recovery %",
                Value = Math.Round(((double)(totalConfirmedCases - (totalFatalCases+totalActiveCases)) / (double)totalConfirmedCases) * 100,2)
            }) ;

            result.Add(new GraphRecord
            {
                Date = lastAvailableDate,
                Key = "Fatal",
                Value = totalFatalCases
            });
            return result;
        }

        private async Task<List<GraphRecord>> GetTestStatsAsync()
        {
            const double keralaPopulationInMillion = 36;
            var casesRecorded = (await TestCasesService.GetCummilativeCases()).ToList();
            var casesPerDay = new[] { casesRecorded[0] }.Concat(casesRecorded.Zip(casesRecorded.Skip(1), (first, second) => new CaseStatus { Count = second.Count - first.Count, Date = second.Date, District = second.District }));
            var result = new List<GraphRecord>();

            result.Add(new GraphRecord
            {
                Date = casesRecorded.Max(x => x.Date),
                Key = "Total Tests",
                Value = casesRecorded.Max(x => x.Count)
            });

            result.Add(new GraphRecord
            {
                Date = casesRecorded.Max(x => x.Date),
                Key = "Tests Per Million",
                Value = Math.Round(casesRecorded.Max(x => x.Count)/ keralaPopulationInMillion)
            });

            result.Add(new GraphRecord
            {
                Date = casesRecorded.Max(x => x.Date),
                Key = "Av. Tests(Last 5 Days)",
                Value = casesPerDay.OrderByDescending(x=>x.Date).Take(5).Average(x => x.Count)
            });

            result.Add(new GraphRecord
            {
                Date = casesRecorded.Max(x => x.Date),
                Key = "Max Tests On One Day",
                Value = casesPerDay.Skip(1).Max(x=>x.Count)
            });

            result.Add(new GraphRecord
            {
                Date = casesRecorded.Last().Date,
                Key = "Tests on Last Day",
                Value = casesPerDay.Last().Count
            });
            return result;
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
        public GenericC19Service<ActiveCaseService> ActiveCaseService { get; set; } = new GenericC19Service<ActiveCaseService>();
        public GenericC19Service<FatalCaseService> FatalCaseService { get; set; } = new GenericC19Service<FatalCaseService>();
    }
}
