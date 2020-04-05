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
            var statusRead = await C19Service.GetCummilativeCases();
            DrawGraph(statusRead);
        }

        private PlotModel CreateDistrictLineChartModel(IEnumerable<CaseStatus> status)
        {
            var plotModel = CreateBaseModel();
            var colors = typeof(OxyColors)
                             .GetFields(BindingFlags.Static | BindingFlags.Public)
                             .Where(f => f.FieldType == typeof(OxyColor))
                             .Select(f => f.GetValue(null))
                             .Cast<OxyColor>()
                             .ToList();

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
                    Color = colors[(int)district.Key],
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 3,
                    MarkerFill = colors[(int)district.Key],
                    Title = district.Key.ToString(),
                };

                plotModel.Series.Add(lineSeries);
            }
            return plotModel;
        }

        private PlotModel CreateStateLineChartModel(IEnumerable<CaseStatus> status)
        {
            var plotModel = CreateBaseModel();
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
            plotModel.Series.Add(lineSeries);
            return plotModel;
        }
        public void DrawGraph(IEnumerable<CaseStatus> status)
        {
            CreatePlotController();
            DistrictLineChartModel = CreateDistrictLineChartModel(status);
            StateLineChartModel = CreateStateLineChartModel(status);
            NotifyOfPropertyChange(nameof(ChartController));
            NotifyOfPropertyChange(nameof(DistrictLineChartModel));
            NotifyOfPropertyChange(nameof(StateLineChartModel));
        }

        private PlotModel CreateBaseModel()
        {
            var plotModel = new PlotModel();
            var xAxis = new DateTimeAxis 
            { 
                Position = AxisPosition.Bottom, 
                StringFormat = "dd MMM",
                CropGridlines = true,
            };
            

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
            plotModel.PlotAreaBorderThickness = new OxyThickness(1, 0, 0, 1);
            plotModel.LegendPlacement = LegendPlacement.Outside;
            plotModel.LegendBorderThickness = 1;
            plotModel.LegendBorder = OxyColors.Black;
            return plotModel;
        }

        private void CreatePlotController()
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

                view.ActualModel.InvalidatePlot(true);
            });

            ChartController = new PlotController();
            ChartController.BindMouseEnter(TrackSeries);
        }

        public PlotController ChartController { get; set; }
        public static IViewCommand<OxyMouseEventArgs> TrackSeries { get; private set; }
        public PlotModel DistrictLineChartModel { get; set; }

        public PlotModel StateLineChartModel { get; set; }

        public GenericC19Service<ActiveCaseService> C19Service { get; set; } = new GenericC19Service<ActiveCaseService>();
    }

   
}
