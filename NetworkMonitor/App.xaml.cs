using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NetworkMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //StartupUri = new Uri(@"ViewModel/NetworkWindow.xaml", UriKind.Relative);
            StartupUri = new Uri(@"MainWindow.xaml", UriKind.Relative);

        }
    }
}
