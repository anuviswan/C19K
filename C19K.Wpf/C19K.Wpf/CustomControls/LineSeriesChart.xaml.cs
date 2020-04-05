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

        public List<CaseStatus> DataCollection
        {
            get { return (List<CaseStatus>)GetValue(DataCollectionProperty); }
            set { SetValue(DataCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataCollectionProperty = DependencyProperty.Register("DataCollection", typeof(List<CaseStatus>), typeof(LineSeriesChart), new PropertyMetadata(Enumerable.Empty<CaseStatus>().ToList(), new PropertyChangedCallback(OnDataPropertyChanged)));

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private static void OnDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as LineSeriesChart;
            instance.UpdatePlotModel();
        }

        public void UpdatePlotModel()
        {
            CurrentPlotModel = LoadChart();
            RaisePropertyChanged(nameof(CurrentPlotModel));
            CurrentPlotModel?.InvalidatePlot(true);
            UpdateLayout();
        }

        public PlotModel CurrentPlotModel { get; set; } = new PlotModel();

        private PlotModel LoadChart()
        {
            if (DataCollection== null || DataCollection.Count() == 0) return default;
            var plotModel = CreateBaseLineSeriesPlotModel();

            foreach (var district in DataCollection.GroupBy(x => x.District)
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
                    Title = district.Key.ToString(),
                    LineStyle = LineStyle.Solid,
                    LineJoin = LineJoin.Round
                };

                plotModel.Series.Add(lineSeries);
            }
            return plotModel;
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
    }
}
