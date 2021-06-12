using System;
using System.Windows;
using SharpPcap.LibPcap;
using System.Collections;
using NetworkMonitor.ViewModel;

namespace NetworkMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CaptureDevice();
        }

        public void CaptureDevice()
        {
            ArrayList list = new ArrayList();

            var devices = LibPcapLiveDeviceList.Instance;

            if (devices.Count < 1)
            {
                MessageBox.Show("No Device");
            }
            foreach (var dev in devices)
            {
                list.Add(Convert.ToString(dev.Interface.FriendlyName));
            }
            DeviceList.ItemsSource = list;
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {
            new NetworkWindow(DeviceList.SelectedItem.ToString()).Show();
        }
    }
}
