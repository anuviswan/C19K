using C19K.Wpf.Attributes;
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
        public List<CaseStatus> DistrictWiseCummilativeCases { get; set; }
        public List<CaseStatus> StateWideCummilativeCases { get; set; }
        public List<CaseStatus> StateWideDailyCases { get; set; }
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
        }

        private async Task<List<CaseStatus>> GetStateWideDailyCasesAsync()
        {
            var casesRecorded = await C19Service.GetDailyCases();
            return casesRecorded.Where(x => x.District == District.State).ToList();
        }

        private async Task<List<CaseStatus>> GetDistrictWiseCummilativeCasesAsync()
        {
            var casesRecorded = await C19Service.GetCummilativeCases();
            return casesRecorded.Where(x=>x.District!= District.State).ToList();
        }

        private async Task<List<CaseStatus>> GetStateWideCummilativeCasesAsync()
        {
            var casesRecorded = await C19Service.GetCummilativeCases();
            return casesRecorded.Where(x => x.District == District.State).ToList();
        }

        public GenericC19Service<HistoryOfCasesService> C19Service { get; set; } = new GenericC19Service<HistoryOfCasesService>();
    }
}
