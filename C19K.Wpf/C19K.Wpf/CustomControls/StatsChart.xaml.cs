using C19K.Wpf.Models;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for StatsChart.xaml
    /// </summary>
    public partial class StatsChart : UserControl
    {
        public StatsChart()
        {
            InitializeComponent();
            DataContext = this;
        }

        public List<GraphRecord> DataCollection
        {
            get { return (List<GraphRecord>)GetValue(DataCollectionProperty); }
            set { SetValue(DataCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataCollectionProperty =
            DependencyProperty.Register(nameof(DataCollection), typeof(List<GraphRecord>), typeof(StatsChart), new PropertyMetadata(Enumerable.Empty<GraphRecord>().ToList(), OnPropertyChanged));

        public string GraphTitle
        {
            get { return (string)GetValue(GraphTitleProperty); }
            set { SetValue(GraphTitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GraphTitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GraphTitleProperty =
            DependencyProperty.Register(nameof(GraphTitle), typeof(string), typeof(StatsChart), new PropertyMetadata(string.Empty, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as StatsChart;
            instance.UpdateLayout();
        }


    }
}
