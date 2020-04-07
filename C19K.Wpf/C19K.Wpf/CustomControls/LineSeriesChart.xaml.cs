using C19K.Wpf.ExtensionMethods;
using C19K.Wpf.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace C19K.Wpf.CustomControls
{
    /// <summary>
    /// Interaction logic for LineSeriesChart.xaml
    /// </summary>
    public partial class LineSeriesChart : UserControl, INotifyPropertyChanged
    {
        public LineSeriesChart()
        {
            InitializeComponent();
        }

        private const string LinearAxisKey = "LinearAxisKey";
        private const string LogarithmicAxisKey = "LogarithmicAxisKey";

        public bool ShowLogarithmicAxis
        {
            get { return (bool)GetValue(ShowLogarithmicAxisProperty); }
            set { SetValue(ShowLogarithmicAxisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowLogarithmicAxis.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowLogarithmicAxisProperty =
            DependencyProperty.Register("ShowLogarithmicAxis", typeof(bool), typeof(LineSeriesChart), new PropertyMetadata(false, OnPropertyChanged));



        public string GraphTitle
        {
            get { return (string)GetValue(GraphTitleProperty); }
            set { SetValue(GraphTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GraphTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraphTitleProperty =
            DependencyProperty.Register("GraphTitle", typeof(string), typeof(LineSeriesChart), new PropertyMetadata(string.Empty,OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as LineSeriesChart;
            instance.UpdatePlotModel();
        }

        public List<GraphRecord> DataCollection
        {
            get { return (List<GraphRecord>)GetValue(DataCollectionProperty); }
            set { SetValue(DataCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataCollectionProperty = DependencyProperty.Register("DataCollection", typeof(List<GraphRecord>), typeof(LineSeriesChart), new PropertyMetadata(Enumerable.Empty<GraphRecord>().ToList(), new PropertyChangedCallback(OnPropertyChanged)));

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public void UpdatePlotModel()
        {
            CurrentPlotModel = LoadChart();

            if (CurrentPlotModel!=null && !string.IsNullOrWhiteSpace(GraphTitle))
            {
                CurrentPlotModel.Title = GraphTitle;
            }
            RaisePropertyChanged(nameof(CurrentPlotModel));
            CurrentPlotModel?.InvalidatePlot(true);
            UpdateLayout();
        }

        public PlotModel CurrentPlotModel { get; set; } = new PlotModel();

        private PlotModel LoadChart()
        {
            if (DataCollection== null || DataCollection.Count() == 0) return default;
            var plotModel = CreateBaseLineSeriesPlotModel();
            var linearSeries =  LoadSeries(plotModel);
            plotModel.Series.AddRange(AssignLinearSeriesAxis(linearSeries));


            if (ShowLogarithmicAxis)
            {
                var logarithmicAxis = LoadSeries(plotModel);
                plotModel.Series.AddRange(AssignLogarithmicAxis(logarithmicAxis));
            }
            return plotModel;
        }

        private IEnumerable<LineSeries> AssignLogarithmicAxis(IEnumerable<LineSeries> lineSeries)
        {
            foreach (var ls in lineSeries)
            {
                if (ShowLogarithmicAxis)
                {
                    ls.Title = "Logarithmic Progress";
                }
                ls.YAxisKey = LogarithmicAxisKey;
                yield return ls;
            }
        }

        private IEnumerable<LineSeries> AssignLinearSeriesAxis(IEnumerable<LineSeries> lineSeries)
        {
            foreach (var ls in lineSeries)
            {
                if (ShowLogarithmicAxis)
                {
                    ls.Title = "Linear Progress";
                }
                ls.YAxisKey = LinearAxisKey;
                yield return ls;
            }
        }

        private IEnumerable<LineSeries> LoadSeries(PlotModel plotModel)
        {
            foreach (var district in DataCollection.GroupBy(x => x.Key)
                                           .OrderBy(x => x.Key))
            {
                var lineSeries = new LineSeries
                {
                    ItemsSource = district.ToList()
                                          .Where(x => x.Value > 0)
                                          .OrderBy(x => x.Date)
                                          .Select(x => new DataPoint(DateTimeAxis.ToDouble(x.Date), x.Value)),
                    MarkerType = MarkerType.Circle,
                    MarkerSize = 3,
                    Title = district.Key.ToString(),
                    LineStyle = LineStyle.Solid,
                    LineJoin = LineJoin.Round,
                };

                yield return lineSeries;
            }
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
                Key = LinearAxisKey
            };

            if (ShowLogarithmicAxis)
            {
                var yAxisLogarithmicAxis = new LogarithmicAxis
                {
                    Position = AxisPosition.Right,
                    MajorGridlineStyle = LineStyle.Solid,
                    MajorGridlineColor = OxyColors.LightGray,
                    Key = LogarithmicAxisKey,
                };
                plotModel.Axes.Add(yAxisLogarithmicAxis);
            }

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);
            plotModel.PlotAreaBorderThickness = ShowLogarithmicAxis ? new OxyThickness(1, 0, 1, 1): new OxyThickness(1, 0, 0, 1);
            plotModel.LegendPlacement = LegendPlacement.Outside;
            plotModel.LegendBorderThickness = 1;
            plotModel.LegendBorder = OxyColors.Black;
            return plotModel;
        }
    }
}
