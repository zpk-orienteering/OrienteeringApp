using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using DAL.Models;
using Orienteering.Helpers;
using System.Threading;

namespace Orienteering
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (!BaseModel.DBSelfTest(DBHelper.ConnectionString))
            {
                DBHelper.IsSelfTestCompleted = false;
                MessageBox.Show("Brak pliku z bazą danych niezbędną do prawidłowego działania programu");
            }
        }
    }
}
