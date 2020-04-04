﻿using C19K.Wpf.Models;
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

namespace C19K.Wpf.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private bool showDistrictDetails;
        private IEnumerable<Status> ActiveCases { get; }
        private IEnumerable<Status> HistoryCases { get; }

        public ShellViewModel()
        {
            Items.Add(new HistoryCaseReportViewModel());
            Items.Add(new ActiveCaseReportViewModel());
            ActivateItem(Items[0]);
        }

        public PlotController ChartController { get; set; }
        public static IViewCommand<OxyMouseEventArgs> TrackSeries { get; private set; }


        public void DrawGraph()
        {
            DrawActiveCases(ActiveCases);
            DrawHistoryCases(HistoryCases);
        }
        public void DrawActiveCases(IEnumerable<Status> status)
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
                    
                    if(series is LineSeries linesSeries)
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

        public void DrawHistoryCases(IEnumerable<Status> status)
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

                HistoryCasesGraphModel.InvalidatePlot(true);
            });

            ChartController = new PlotController();
            ChartController.BindMouseEnter(TrackSeries);

            HistoryCasesGraphModel = new PlotModel();
            HistoryCasesGraphModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "dd MMM" });
            HistoryCasesGraphModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

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

                    HistoryCasesGraphModel.Series.Add(lineSeries);
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
                HistoryCasesGraphModel.Series.Add(lineSeries);
            }

            NotifyOfPropertyChange(nameof(HistoryCasesGraphModel));
        }

        public PlotModel ActiveCaseGraphModel { get; set; }
        public PlotModel HistoryCasesGraphModel { get; set; }

        public List<IReportViewModel> ViewModelCollection { get; set; } = new List<IReportViewModel>();
        public bool ShowDistrictDetails
        {
            get => showDistrictDetails;
            set
            {
                showDistrictDetails = value;
                DrawActiveCases(ActiveCases);
            }
        }
    }
}
