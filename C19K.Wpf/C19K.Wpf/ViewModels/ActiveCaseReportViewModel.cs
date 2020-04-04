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
using System.Text;
using System.Threading.Tasks;

namespace C19K.Wpf.ViewModels
{
    [ReportDescriptionAttribute(Description = "Active Cases In Kerala", Title = "Active Cases")]
    public class ActiveCaseReportViewModel:Screen,IReportViewModel<ActiveCaseService>
    {
        private IEnumerable<Status> ActiveCases { get; }
        public ActiveCaseReportViewModel()
        {
            DisplayName = "Active Cases";
            ActiveCases = C19Service.Get();
            DrawGraph(ActiveCases);
        }

        public Task Reload()
        {
            DrawGraph(ActiveCases);
            return Task.CompletedTask;
        }
        public void DrawGraph(IEnumerable<Status> status)
        {
            TrackSeries = new DelegatePlotCommand<OxyMouseEventArgs>((view, controller, args) =>
            {
                if (view.ActualModel.Series.Cast<LineSeries>().Any(x => x.Color == OxyColors.Red))
                {
                    var selectedSeries = view.ActualModel.Series.Cast<LineSeries>().Single(x => x.Color == OxyColors.Red);
                    selectedSeries.Color = OxyColors.LightBlue;
                }

                var series = view.ActualModel.GetSeriesFromPoint(args.Position);
                if (series != null)
                {

                    if (series is LineSeries linesSeries)
                    {
                        linesSeries.Color = OxyColors.Red;

                    }
                }

                ActiveCaseGraphModel.InvalidatePlot(true);
            });

            ChartController = new PlotController();
            ChartController.BindMouseEnter(TrackSeries);

            ActiveCaseGraphModel = new PlotModel();
            ActiveCaseGraphModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "dd MMM" });
            ActiveCaseGraphModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

            if (ShowDistrictDetails)
            {
                foreach (var district in status.GroupBy(x => x.District)
                                               .Where(x => x.Key != District.State)
                                               .OrderBy(x => x.Key))
                {
                    var lineSeries = new LineSeries
                    {
                        ItemsSource = district.ToList()
                                              .Where(x => x.Count > 0)
                                              .OrderBy(x => x.Date)
                                              .Select(x => new DataPoint(DateTimeAxis.ToDouble(x.Date), x.Count)),
                        Color = OxyColors.LightBlue,
                        MarkerType = MarkerType.Circle,
                        MarkerSize = 3,
                        MarkerFill = OxyColors.LightBlue,
                        Title = District.State.ToString(),
                    };

                    ActiveCaseGraphModel.Series.Add(lineSeries);
                }
            }
            else
            {
                var lineSeries = new LineSeries
                {
                    ItemsSource = status.Where(x => x.District == District.State)
                                        .Where(x => x.Count > 0)
                                        .OrderBy(x => x.Date)
                                        .Select(x => new DataPoint(DateTimeAxis.ToDouble(x.Date), x.Count)),
                    Color = OxyColors.LightBlue,
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 3,
                    MarkerFill = OxyColors.LightBlue,
                    Title = District.State.ToString(),
                };
                ActiveCaseGraphModel.Series.Add(lineSeries);
            }

            NotifyOfPropertyChange(nameof(ActiveCaseGraphModel));
        }

        public PlotController ChartController { get; set; }
        public static IViewCommand<OxyMouseEventArgs> TrackSeries { get; private set; }
        public PlotModel ActiveCaseGraphModel { get; set; }
        private bool showDistrictDetails;
        public bool ShowDistrictDetails
        {
            get => showDistrictDetails;
            set
            {
                showDistrictDetails = value;
                DrawGraph(ActiveCases);
            }
        }

        public GenericC19Service<ActiveCaseService> C19Service { get; set; } = new GenericC19Service<ActiveCaseService>();
    }

   
}
