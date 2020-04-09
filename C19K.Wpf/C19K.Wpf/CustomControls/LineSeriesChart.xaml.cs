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

        public AxisType PrimaryAxisType
        {
            get { return (AxisType)GetValue(PrimaryAxisTypeProperty); }
            set { SetValue(PrimaryAxisTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PrimaryAxisType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrimaryAxisTypeProperty =
            DependencyProperty.Register("PrimaryAxisType", typeof(AxisType), typeof(LineSeriesChart), new PropertyMetadata(AxisType.Linear,OnPropertyChanged));


        public AxisType SecondaryAxisType
        {
            get { return (AxisType)GetValue(SecondaryAxisTypeProperty); }
            set { SetValue(SecondaryAxisTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SecondaryAxisType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondaryAxisTypeProperty =
            DependencyProperty.Register("SecondaryAxisType", typeof(AxisType), typeof(LineSeriesChart), new PropertyMetadata(AxisType.Linear, OnPropertyChanged));



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
        public static readonly DependencyProperty DataCollectionProperty = DependencyProperty.Register("DataCollection", typeof(List<GraphRecord>), 
            typeof(LineSeriesChart), new PropertyMetadata(null, new PropertyChangedCallback(OnPropertyChanged)));


        public List<GraphRecord> SecondaryDataCollection
        {
            get { return (List<GraphRecord>)GetValue(SecondaryDataCollectionProperty); }
            set { SetValue(SecondaryDataCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SecondaryDataCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondaryDataCollectionProperty =
            DependencyProperty.Register("SecondaryDataCollection", typeof(List<GraphRecord>), 
                typeof(LineSeriesChart), new PropertyMetadata(null, new PropertyChangedCallback(OnPropertyChanged)));



        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private bool IsSecondaryAxisEnabled => SecondaryDataCollection != null;

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
            var plotModel = CreateBaseLineSeriesPlotModel(IsSecondaryAxisEnabled);
            
            var primarySeries =  CreatePrimaryAxisSeries();
            plotModel.Series.AddRange(primarySeries);

            if (IsSecondaryAxisEnabled)
            {
                var secondarySeries = CreateSecondaryAxisSeries();
                plotModel.Series.AddRange(secondarySeries);
            }
            return plotModel;
        }

        private IEnumerable<LineSeries> CreatePrimaryAxisSeries()
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
                    YAxisKey = PrimaryAxisType == AxisType.Linear ? LinearAxisKey : LogarithmicAxisKey
                };

                yield return lineSeries;
            }
        }

        private IEnumerable<LineSeries> CreateSecondaryAxisSeries()
        {
            foreach (var district in SecondaryDataCollection.GroupBy(x => x.Key)
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
                    YAxisKey = SecondaryAxisType == AxisType.Linear ? LinearAxisKey : LogarithmicAxisKey
            };

                yield return lineSeries;
            }
        }

        private PlotModel CreateBaseLineSeriesPlotModel(bool isSecondaryAxisEnabled = false)
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

            var primaryAxis = PrimaryAxisType == AxisType.Linear ? CreateLinearAxis() : CreateLogarithmicAxis();

            if (isSecondaryAxisEnabled)
            {
                var secondaryAxis = SecondaryAxisType == AxisType.Linear ? CreateLinearAxis() : CreateLogarithmicAxis();
                plotModel.Axes.Add(secondaryAxis);
            }
            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(primaryAxis);
            plotModel.PlotAreaBorderThickness = isSecondaryAxisEnabled ? new OxyThickness(1, 0, 1, 1): new OxyThickness(1, 0, 0, 1);
            plotModel.LegendPlacement = LegendPlacement.Outside;
            plotModel.LegendBorderThickness = 1;
            plotModel.LegendBorder = OxyColors.Black;
            return plotModel;
        }

        private Axis CreateLinearAxis()
        {
            return new LinearAxis
            {
                Position = AxisPosition.Left,
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColors.LightGray,
                Key = LinearAxisKey
            }; 
        }

        private Axis CreateLogarithmicAxis()
        {
            return new LogarithmicAxis
            {
                Position = AxisPosition.Right,
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColors.LightGray,
                Key = LogarithmicAxisKey,
            }; 
        }
    }
}
