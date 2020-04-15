using C19K.Wpf.Models;
using Caliburn.Micro;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using C19K.Wpf.Service;
using C19K.Wpf.Attributes;

namespace C19K.Wpf.ViewModels
{
    public class ShellViewModel : Conductor<IReportViewModel>.Collection.OneActive
    {
        private bool showDistrictDetails;
        private IEnumerable<GraphRecord> ActiveCases { get; }
        private IEnumerable<GraphRecord> HistoryCases { get; }

        public ShellViewModel()
        {
            LoadReports();
            ActivateItem(Items[1]);
        }

        private void LoadReports()
        {
            var reportTypes = this.GetType()
                                  .Assembly
                                  .GetTypes()
                                  .Where(x => typeof(IReportViewModel).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface);

            foreach (var type in reportTypes)
            {
                var instance = IoC.GetInstance(type,null) as IReportViewModel;
                var displayProperties = type.GetAttributes<ReportDescriptionAttribute>(false).Single();
                instance.DisplayName = displayProperties.Title;
                Items.Add(instance);
            }
        }

        public string LastUpdatedInformation => Task.Run(async () => await GetLastUpdatedInformation()).Result;

        private async Task<string> GetLastUpdatedInformation()
        {
            var historicalCasesService = IoC.Get<GenericC19Service<HistoryOfCasesService>>();
            var lastUpdatedDate = (await historicalCasesService.GetCummilativeCases()).Max(x => x.Date);
            return $"Last updated on {lastUpdatedDate.ToString("dd MMM,yyyy")}";

        }

    }
}
