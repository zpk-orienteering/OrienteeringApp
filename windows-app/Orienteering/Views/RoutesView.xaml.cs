using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Orienteering.ViewModels;

namespace Orienteering.Views
{
    /// <summary>
    /// Interaction logic for RoutesView.xaml
    /// </summary>
    public partial class RoutesView : UserControl
    {
        public RoutesView()
        {
            InitializeComponent();
            this.DataContext = RouteViewModel.GetInstance();
        }

        private void TabItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void TabItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void tabControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void tabControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (this.DataContext as RouteViewModel).AddControlPoint.Execute(null);
        }

        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            (this.DataContext as RouteViewModel).DeleteControlPoint.Execute(null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 1;
        }

    }
}
