using C19K.Wpf.Attributes;
using C19K.Wpf.ExtensionMethods;
using C19K.Wpf.Models;
using C19K.Wpf.Service;
using Caliburn.Micro;
using MahApps.Metro;
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
        public double DefaultTileValueFontSize => 48;
        public double DefaultTileTitleFontSize => 16;
        public Color DefaultTileForeColor => Colors.White;
        public Color DefaultTitleBackgroundColor { get; set; }
        public List<GraphRecord> NumberOfDaysForMajorMilestones { get; set; }

        public TileRecord TotalConfirmedCases { get; set; }
        public TileRecord TotalActiveCases { get; set; }
        public TileRecord TotalFatalCases { get; set; }
        public TileRecord RecoveryRate { get; set; }
        public HistoryCaseReportViewModel()
        {
            DisplayName = "History";
            var brush = (Brush)ThemeManager.GetAccent("Blue").Resources["AccentColorBrush"];
            DefaultTitleBackgroundColor = ((SolidColorBrush)brush).Color;
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
            NumberOfDaysForMajorMilestones = await GetNumberOfDaysForMajorMilestonesAsync();
            TotalConfirmedCases = await GetTotalConfirmedCase();
            TotalActiveCases = await GetTotalActiveCases();
            TotalFatalCases = await GetTotalFatalCases();
            RecoveryRate = await GetRecoveryRate();

            RaisePropertyChangedEventForTileProperties();
        }

        private void RaisePropertyChangedEventForTileProperties()
        {
            NotifyOfPropertyChange(nameof(DefaultTitleBackgroundColor));
            NotifyOfPropertyChange(nameof(DefaultTileForeColor));
            NotifyOfPropertyChange(nameof(TotalConfirmedCases));
            NotifyOfPropertyChange(nameof(DefaultTileValueFontSize));
            NotifyOfPropertyChange(nameof(DefaultTileTitleFontSize));
        }

        public async Task<TileRecord> GetTotalConfirmedCase()
        {
            var confirmedTotalCases = await HistoryOfCasesService.GetCummilativeCases();
            return new TileRecord
            {
                Value = confirmedTotalCases.Where(x => x.District == District.State).Max(x => x.Count),
                Title = "Total Confirmed Cases"
            };
        }

        public async Task<TileRecord> GetTotalActiveCases()
        {
            var totalActiveCases = await ActiveCaseService.GetCummilativeCases();
            return new TileRecord
            {
                Value = totalActiveCases.Where(x => x.District == District.State).OrderByDescending(x => x.Date).First().Count,
                Title = "Total Active Cases"
            };
        }

        public async Task<TileRecord> GetRecoveryRate()
        {
            var activeCasesCummilative = await ActiveCaseService.GetCummilativeCases();
            var historicalCasesCummilative = await HistoryOfCasesService.GetCummilativeCases();
            var fatalCaseCummilative = await FatalCaseService.GetCummilativeCases();

            var totalConfirmedCases = historicalCasesCummilative.Where(x => x.District == District.State).OrderBy(x => x.Date).Last().Count;
            var totalActiveCases = activeCasesCummilative.Where(x => x.District == District.State).OrderBy(x => x.Date).Last().Count;
            var totalFatalCases = fatalCaseCummilative.Where(x => x.District == District.State).Max(x => x.Count);

            return new TileRecord
            {
                Title = "Recovery %",
                Value = Math.Round(((double)(totalConfirmedCases - (totalFatalCases + totalActiveCases)) / (double)totalConfirmedCases) * 100, 2)
            };
        }

        public async Task<TileRecord> GetTotalFatalCases()
        {
            var totalFatalCases = await FatalCaseService.GetCummilativeCases();
            return new TileRecord
            {
                Value = totalFatalCases.Where(x => x.District == District.State).OrderByDescending(x => x.Date).First().Count,
                Title = "Total Deaths"
            };
        }

        private async Task<List<GraphRecord>> GetNumberOfDaysForMajorMilestonesAsync()
        {
            var historicalCasesCummilative = (await HistoryOfCasesService.GetCummilativeCases()).Where(x=>x.District == District.State).OrderBy(x=>x.Date);
            var numberTillFifty = historicalCasesCummilative.TakeWhile(x => x.Count <= 50);
            var numberFromFiftyToHundred = historicalCasesCummilative.Skip(numberTillFifty.Count()).TakeWhile(x => x.Count <= 100);
            var numberFromHundredToTwoHundred = historicalCasesCummilative.Skip(numberFromFiftyToHundred.Count()).TakeWhile(x => x.Count <= 200);
            var numberFromTwoHundredToThreeHundred = historicalCasesCummilative.Skip(numberFromHundredToTwoHundred.Count()).TakeWhile(x => x.Count <= 300);
            var numberFromThreeHundredToFourHundred = historicalCasesCummilative.Skip(numberFromTwoHundredToThreeHundred.Count()).TakeWhile(x => x.Count <= 400);

            var result = new List<GraphRecord>();

            result.Add(new GraphRecord
            {
                Date = numberFromFiftyToHundred.Last().Date,
                Key = "50-100",
                Value = numberFromFiftyToHundred.Count()
            });

            result.Add(new GraphRecord
            {
                Date = numberFromHundredToTwoHundred.Last().Date,
                Key = "100-200",
                Value = numberFromHundredToTwoHundred.Count()
            });

            result.Add(new GraphRecord
            {
                Date = numberFromTwoHundredToThreeHundred.Last().Date,
                Key = "200-300",
                Value = numberFromTwoHundredToThreeHundred.Count()
            });

            result.Add(new GraphRecord
            {
                Date = numberFromThreeHundredToFourHundred.Last().Date,
                Key = "300-400",
                Value = numberFromThreeHundredToFourHundred.Count()
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
