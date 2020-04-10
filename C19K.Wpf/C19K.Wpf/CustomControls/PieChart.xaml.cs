using C19K.Wpf.Models;
using OxyPlot;
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
    /// Interaction logic for PieChartControl.xaml
    /// </summary>
    public partial class PieChart : UserControl, INotifyPropertyChanged
    {
        public PieChart()
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
            DependencyProperty.Register("GraphTitle", typeof(string), typeof(PieChart), new PropertyMetadata(string.Empty, OnPropertyChanged));

        public List<GraphRecord> DataCollection
        {
            get { return (List<GraphRecord>)GetValue(DataCollectionProperty); }
            set { SetValue(DataCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataCollectionProperty = DependencyProperty.Register("DataCollection", typeof(List<GraphRecord>),
            typeof(PieChart), new PropertyMetadata(Enumerable.Empty<GraphRecord>().ToList(), new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as PieChart;
            instance.UpdatePlotModel();
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public PlotModel CurrentPlotModel { get; set; } = new PlotModel();

        private PlotModel LoadChart()
        {
            if (DataCollection == null || DataCollection.Count() == 0) return default;

            var plotModel = new PlotModel();
            var seriesP1 = new PieSeries 
            { 
                StrokeThickness = 2.0, 
                AngleSpan = 360, 
                StartAngle = 0,
                InsideLabelColor = OxyColors.White,
                InsideLabelPosition = 0.8,
                AreInsideLabelsAngled = true
            };

            foreach(var item in DataCollection)
            {
                seriesP1.Slices.Add(new PieSlice(item.Key, item.Value));
            }
            plotModel.Series.Add(seriesP1);
            return plotModel;
        }



    }
}
