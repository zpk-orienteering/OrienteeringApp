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
    /// Interaction logic for QRCodeView.xaml
    /// </summary>
    public partial class QRCodeView : UserControl
    {
        public QRCodeView()
        {
            InitializeComponent();
            this.DataContext = QrCodeViewModel.GetInstance();
        }

        private void TabControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void TabControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void btnDodaj_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = 1;
        }
    }
}
