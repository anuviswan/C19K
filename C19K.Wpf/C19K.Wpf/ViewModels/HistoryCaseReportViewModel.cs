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
    [ReportDescriptionAttribute(Description = "Total Cases In Kerala", Title = "Total Cases")]
    public class HistoryCaseReportViewModel:Screen,IReportViewModel<HistoryOfCasesService>
    {
        private IEnumerable<Status> ActiveCases { get; }
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
            var statusRead = await C19Service.Get();
            DrawGraph(statusRead);
        }

        private PlotModel CreateDistrictLineChartModel(IEnumerable<Status> status)
        {
            var plotModel = CreateBaseLineSeriesPlotModel();

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
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 3,
                    LineStyle = LineStyle.Solid,
                    LineJoin = LineJoin.Round,
                    Title = district.Key.ToString(),
                };

                plotModel.Series.Add(lineSeries);
            }
            return plotModel;
        }

        private PlotModel CreateStateLineChartModel(IEnumerable<Status> status)
        {
            var plotModel = CreateBaseLineSeriesPlotModel();
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
        public void DrawGraph(IEnumerable<Status> status)
        {
            CreatePlotController();
            DistrictLineChartModel = CreateDistrictLineChartModel(status);
            StateLineChartModel = CreateStateLineChartModel(status);
            DailyBarChartModel = CreateDailyColumnGraph(status);
            NotifyOfPropertyChange(nameof(ChartController));
            NotifyOfPropertyChange(nameof(DistrictLineChartModel));
            NotifyOfPropertyChange(nameof(StateLineChartModel));
            NotifyOfPropertyChange(nameof(DailyBarChartModel));
        }

        private PlotModel CreateDailyColumnGraph(IEnumerable<Status> status)
        {
            var model = new PlotModel()
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.BottomCenter,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendBorderThickness = 0
            };

            var categoryAxis = new CategoryAxis { Position = AxisPosition.Bottom };
            categoryAxis.Labels.AddRange(status.Where(x => x.District == District.State).OrderBy(x => x.Date).Select(x => x.Date.ToString("dd-MMM")));
            ColumnSeries s1 = new ColumnSeries();
            s1.Items.AddRange(status.Where(x => x.District == District.State).OrderBy(x => x.Date).Select(x => new ColumnItem( x.DailyCount)));
            model.Axes.Add(categoryAxis);
            model.Series.Add(s1);
            return model;
        }
        private PlotModel CreateBaseLineSeriesPlotModel()
        {
            var plotModel = new PlotModel();

            var xAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "dd MMM",
                CropGridlines = true,
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColors.LightGray,

            };

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColors.LightGray,
            };

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);
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

        public PlotModel DailyBarChartModel { get; set; }

        public GenericC19Service<HistoryOfCasesService> C19Service { get; set; } = new GenericC19Service<HistoryOfCasesService>();
    }
}
