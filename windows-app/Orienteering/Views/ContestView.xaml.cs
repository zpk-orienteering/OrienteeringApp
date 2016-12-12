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
    /// Interaction logic for ContestView.xaml
    /// </summary>
    public partial class ContestView : UserControl
    {
        public ContestView()
        {
            InitializeComponent();
            this.DataContext = ContestViewModel.GetInstance();
        }

        private void TabItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void tabControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        public void SetControlPanelTab()
        {
            this.tabControl.SelectedIndex = 3;
        }
    }
}
