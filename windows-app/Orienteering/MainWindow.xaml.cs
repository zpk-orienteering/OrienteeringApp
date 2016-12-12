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
using DAL.Models;
using DAL;
using Orienteering.Views.PartialViews;
using Orienteering.Helpers;

namespace Orienteering
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Elysium.Controls.Window
    {
        private ContestModel model;

        public MainWindow()
        {
            
        }

        void ocw_OnContestLoaded(object sender)
        {
            (sender as OpenContestsWindow).Close();
            sliderPanel.SelectedIndex = 4;
        }

        private void DockPanel_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            sliderPanel.SelectedIndex = 1;
        }

        private void DockPanel_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            sliderPanel.SelectedIndex = 2;
        }

        private void DockPanel_MouseLeftButtonDown_3(object sender, MouseButtonEventArgs e)
        {
            sliderPanel.SelectedIndex = 3;
        }

        private void DockPanel_MouseLeftButtonDown_4(object sender, MouseButtonEventArgs e)
        {
            sliderPanel.SelectedIndex = 4;
        }

        private void TabItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void TabItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void TabControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void TabControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            sliderPanel.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            model = new ContestModel(DBHelper.ConnectionString);
            List<Contest> activeContests = model.GetAllActiveContests();
            if (activeContests != null && activeContests.Count > 0)
            {
                this.infoGrid.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void contestView_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            model = new ContestModel(DBHelper.ConnectionString);
            List<Contest> activeContests = model.GetAllActiveContests();

            if (activeContests != null && activeContests.Count > 0)
            {
                OpenContestsWindow ocw = new OpenContestsWindow(activeContests);
                ocw.OnContestLoaded += new OpenContestsWindow.ContestLoaded(ocw_OnContestLoaded);
                ocw.Show();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            LicenseWindow lw = new LicenseWindow();
            lw.Show();
        }
    }
}
