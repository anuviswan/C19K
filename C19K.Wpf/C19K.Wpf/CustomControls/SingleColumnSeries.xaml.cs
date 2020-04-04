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
    /// Interaction logic for SingleColumnSeries.xaml
    /// </summary>
    public partial class SingleColumnSeries : UserControl, INotifyPropertyChanged
    {
        public SingleColumnSeries()
        {
            InitializeComponent();
            DataContext = this;
        }



        public int MyProperty
        {
            get { return (int)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("MyProperty", typeof(int), typeof(SingleColumnSeries), new FrameworkPropertyMetadata(
            0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,OnNewMemChanged));

        private static void OnNewMemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public List<Status> DataCollection
        {
            get { return (List<Status>)GetValue(DataCollectionProperty); }
            set { SetValue(DataCollectionProperty, value); }
        }

        public string SampleTitle { get; set; } = "sdsadasdasdasdsadasdsad";

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataCollectionProperty = DependencyProperty.Register("DataCollection", typeof(List<Status>), typeof(SingleColumnSeries), new PropertyMetadata(Enumerable.Empty<Status>().ToList(), new PropertyChangedCallback(OnDataPropertyChanged)));

        public event PropertyChangedEventHandler PropertyChanged;

        private static void OnDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as SingleColumnSeries;
            instance.UpdatePlotModel();
        }



        public void UpdatePlotModel()
        {
            CurrentPlotModel = CreateDailyColumnGraph();
            CurrentPlotModel?.InvalidatePlot(true);
            RaisePropertyChanged(nameof(CurrentPlotModel));
            RaisePropertyChanged(nameof(InternalCount));
        }

        public PlotModel CurrentPlotModel { get; set; } = new PlotModel();
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        private PlotModel CreateDailyColumnGraph()
        {
            if (DataCollection.Count == 0) return default;
            if (DataCollection.Select(x => x.District).Distinct().Count() > 1)
                throw new Exception();

            var model = new PlotModel()
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.BottomCenter,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendBorderThickness = 0
            };

            var categoryAxis = new CategoryAxis { Position = AxisPosition.Bottom };
            categoryAxis.Labels.AddRange(DataCollection.OrderBy(x => x.Date).Select(x => x.Date.ToString("dd-MMM")));
            ColumnSeries s1 = new ColumnSeries();
            var dailyCount = DataCollection.OrderBy(x => x.Date).Select(x => x.Count);

            s1.Items.AddRange(dailyCount.Zip(dailyCount.Skip(1), (x, y) => y - x).Select(x => new ColumnItem(x)));
            s1.LabelFormatString = "{0}";
            s1.ToolTip = "{0}";
            model.Axes.Add(categoryAxis);
            model.Series.Add(s1);
            InternalCount = DataCollection.Count;
            
           
            return model;
        }

        public int InternalCount { get; set; }
    }
}
