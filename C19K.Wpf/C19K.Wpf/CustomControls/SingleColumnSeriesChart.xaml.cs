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
    /// Interaction logic for SingleColumnSeriesChart.xaml
    /// </summary>
    public partial class SingleColumnSeriesChart : UserControl, INotifyPropertyChanged
    {
        public SingleColumnSeriesChart()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string GraphTitle
        {
            get { return (string)GetValue(GraphTitleProperty); }
            set { SetValue(GraphTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GraphTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraphTitleProperty =
            DependencyProperty.Register("GraphTitle", typeof(string), typeof(SingleColumnSeriesChart), new PropertyMetadata(string.Empty, GraphTitleChanged));

        private static void GraphTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as SingleColumnSeriesChart;
            instance.UpdatePlotModel();
        }

        public List<GraphRecord> DataCollection
        {
            get { return (List<GraphRecord>)GetValue(DataCollectionProperty); }
            set { SetValue(DataCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataCollectionProperty = DependencyProperty.Register("DataCollection", typeof(List<GraphRecord>), 
            typeof(SingleColumnSeriesChart), new PropertyMetadata(Enumerable.Empty<GraphRecord>().ToList(), new PropertyChangedCallback(OnDataPropertyChanged)));

        

        private static void OnDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as SingleColumnSeriesChart;
            instance.UpdatePlotModel();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void UpdatePlotModel()
        {
            CurrentPlotModel = LoadChart();
            if (CurrentPlotModel != null && !string.IsNullOrWhiteSpace(GraphTitle))
            {
                CurrentPlotModel.Title = GraphTitle;
            }
            RaisePropertyChanged(nameof(CurrentPlotModel));
            CurrentPlotModel?.InvalidatePlot(true);
            UpdateLayout();
        }

        public PlotModel CurrentPlotModel { get; set; } = new PlotModel();
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        private PlotModel LoadChart()
        {
            if (DataCollection ==null || DataCollection.Count == 0) return default;

            if (DataCollection.Select(x => x.Key).Distinct().Count() > 1)
                throw new Exception("More than one District found");

            var model = new PlotModel()
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.BottomCenter,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendBorderThickness = 0,
            };

            var categoryAxis = new CategoryAxis { Position = AxisPosition.Bottom, Angle = -45, GapWidth=0 };
            categoryAxis.Labels.AddRange(DataCollection.OrderBy(x => x.Date).Select(x => x.Date.ToString("dd-MMM")));
            ColumnSeries series = new ColumnSeries();
            series.Items.AddRange(DataCollection.OrderBy(x => x.Date).Select(x => x.Value).Select(x => new ColumnItem(x)));
            series.LabelFormatString = "{0}";
            series.ToolTip = "{0}";
            series.Title = DataCollection.First().Key.ToString();
            model.Axes.Add(categoryAxis);
            model.Series.Add(series);
            return model;
        }
    }
}
