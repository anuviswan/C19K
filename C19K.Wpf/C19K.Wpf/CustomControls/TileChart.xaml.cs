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
    /// Interaction logic for TileChart.xaml
    /// </summary>
    public partial class TileChart : UserControl
    {
        public TileChart()
        {
            InitializeComponent();
            DataContext = this;
        }



        public TileRecord Data
        {
            get { return (TileRecord)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(TileRecord), typeof(TileChart), new PropertyMetadata(new TileRecord(), OnPropertyChanged));


        public Color TileColor
        {
            get { return (Color)GetValue(TileColorProperty); }
            set { SetValue(TileColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TileColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TileColorProperty =
            DependencyProperty.Register("TileColor", typeof(Color), typeof(TileChart), new PropertyMetadata(Colors.Red, OnPropertyChanged));


        public Color FontColor
        {
            get { return (Color)GetValue(FontColorProperty); }
            set { SetValue(FontColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontColorProperty =
            DependencyProperty.Register("FontColor", typeof(Color), typeof(TileChart), new PropertyMetadata(Colors.White, OnPropertyChanged));



        public double TitleFontSize
        {
            get { return (double)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleFontSizeProperty =
            DependencyProperty.Register("TitleFontSize", typeof(double), typeof(TileChart), new PropertyMetadata((double)48, OnPropertyChanged));



        public double ValueFontSize
        {
            get { return (double)GetValue(ValueFontSizeProperty); }
            set { SetValue(ValueFontSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueFontSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueFontSizeProperty =
            DependencyProperty.Register("ValueFontSize", typeof(double), typeof(TileChart), new PropertyMetadata((double)16, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as TileChart;
            instance.UpdateLayout();
        }

    }
}
