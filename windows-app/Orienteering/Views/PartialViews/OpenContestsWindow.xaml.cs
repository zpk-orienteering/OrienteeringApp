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
using DAL;
using Orienteering.ViewModels;

namespace Orienteering.Views.PartialViews
{
    /// <summary>
    /// Interaction logic for OpenContestsWindow.xaml
    /// </summary>
    public partial class OpenContestsWindow : Window
    {
        public delegate void ContestLoaded(object sender);
        public event ContestLoaded OnContestLoaded;

        public OpenContestsWindow(IEnumerable<Contest> aContests)
        {
            InitializeComponent();

            this.contestList.ItemsSource = aContests;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.contestList.SelectedItem != null)
            {
                ContestViewModel.GetInstance().LoadOpenContest((Contest)this.contestList.SelectedItem);
                OnContestLoaded(this);
            }
        }
    }
}
