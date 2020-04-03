using C19K.Wpf.Data;
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

namespace C19K.Wpf.ViewModels
{
    public class ShellViewModel:Screen
    {
        private bool showDistrictDetails;
        private IEnumerable<Status> Status { get; }

        public ShellViewModel()
        {
            var reader = new Reader();
            Status = reader.Read(GetFilePath());
            DrawGraph(Status);
        }

        private string GetFilePath()
        {
            var appSettings = ConfigurationManager.AppSettings;
            return appSettings["CsvPath"] ?? throw new Exception("Missing Configuration: File Path");
        }

        public PlotController ChartController { get; set; }
        public static IViewCommand<OxyMouseEventArgs> TrackSeries { get; private set; }
        public void DrawGraph(IEnumerable<Status> status)
        {
            TrackSeries = new DelegatePlotCommand<OxyMouseEventArgs>((view, controller, args) =>
            {
                controller.AddHoverManipulator(view, new TrackerManipulator(view) { LockToInitialSeries = false, Snap = false, PointsOnly = false }, args);
                
                var series = view.ActualModel.GetSeriesFromPoint(args.Position);
                if (series != null)
                {

                }
            });

            ChartController = new PlotController();
            ChartController.BindMouseEnter(TrackSeries);

            GraphModel = new PlotModel();
            GraphModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "dd MMM" });
            GraphModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

            if (ShowDistrictDetails)
            {
                foreach (var district in status.GroupBy(x => x.District)
                                               .Where(x => x.Key != District.State)
                                               .OrderBy(x => x.Key))
                {
                    var lineSeries = new LineSeries
                    {
                        ItemsSource = district.ToList()
                                              .Where(x => x.ActiveCount > 0)
                                              .OrderBy(x => x.Date)
                                              .Select(x => new DataPoint(DateTimeAxis.ToDouble(x.Date), x.ActiveCount)),
                        Color = OxyColors.LightBlue,
                        MarkerType = MarkerType.Circle,
                        MarkerSize = 3,
                        MarkerFill = OxyColors.LightBlue,
                    };

                    GraphModel.Series.Add(lineSeries);
                }
            }
            else
            {
                var lineSeries = new LineSeries
                {
                    ItemsSource = status.Where(x => x.District == District.State)
                                        .Where(x => x.ActiveCount > 0)
                                        .OrderBy(x => x.Date)
                                        .Select(x => new DataPoint(DateTimeAxis.ToDouble(x.Date), x.ActiveCount)),
                    Color = OxyColors.LightBlue,
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 3,
                    MarkerFill = OxyColors.LightBlue,
                    Title = District.State.ToString(),
                };
                GraphModel.Series.Add(lineSeries);
            }

            NotifyOfPropertyChange(nameof(GraphModel));
        }

        public PlotModel GraphModel { get; set; }

        public bool ShowDistrictDetails
        {
            get => showDistrictDetails;
            set
            {
                showDistrictDetails = value;
                DrawGraph(Status);
            }
        }
    }
}
