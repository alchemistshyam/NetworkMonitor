using System;
using System.Windows;
using MahApps.Metro.Controls;
using NetworkMonitor.Model;
using SharpPcap;
using PacketDotNet;
using SharpPcap.LibPcap;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using NetworkMonitor.Utilities;
using System.Windows.Threading;
using System.Data;
using System.Windows.Media;

namespace NetworkMonitor.ViewModel
{
    /// <summary>
    /// Interaction logic for NetworkWindow.xaml
    /// </summary>
    public partial class NetworkWindow : Window
    {

        public static int Serial { get; set; }
        public string DeviceChoice { get; set; }
        public static SQLiteConnection  connection { get; set; }
        public static int id { get; set; }
        public static SharpPcap.LibPcap.LibPcapLiveDevice device { get; set; }
        public NetworkWindow(string DeviceName)
        {
            DeviceChoice = DeviceName;
            InitializeComponent();
            Initialize();
            NetworkCapture();
            SetDisplayhTimer();
        }

        public void Initialize()
        {
            connection = null;
            id = 1;
            Serial = 1;
            new Database().DeleteData("network");
            connection = new Utilities.Database().GetConnection();
            connection.Open();
        }

        private void SetDisplayhTimer()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(PopulateData);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            dispatcherTimer.Start();
        }
        public void PopulateData(object sender, EventArgs e)
        {

            DataTable networkTable = new Database().GetDataWithQuery("network", "Select * from network where id >=" + id );

            if(networkTable.Rows.Count > 0)
            {
                for (int i = 0; i < networkTable.Rows.Count; i++)
                {
                    Network networkModel = new Network();

                    networkModel.serialNumber = Convert.ToInt32(networkTable.Rows[i]["Serial"]);
                    networkModel.time = Convert.ToDateTime(networkTable.Rows[i]["timestamp"]);
                    networkModel.source = Convert.ToString(networkTable.Rows[i]["source"]);
                    networkModel.destination = Convert.ToString(networkTable.Rows[i]["destination"]);
                    networkModel.protocol = Convert.ToString(networkTable.Rows[i]["protocol"]);
                    networkModel.frameLength = Convert.ToInt32(networkTable.Rows[i]["framelength"]);
                    NetworkMonitorWindow.Items.Add(networkModel);
                    id++;
                }
            }
        }



        public  void NetworkCapture()
        {
            string ver = SharpPcap.Version.VersionString;

            var devices = LibPcapLiveDeviceList.Instance;


            device = devices[0];
            foreach (var dev in devices)
            {
                if(dev.Interface.FriendlyName.Equals(DeviceChoice))
                {
                    device = dev;
                    break;
                }
            }


            device.OnPacketArrival +=
                new PacketArrivalEventHandler(device_OnPacketArrival);

            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

            device.StartCapture();

            //device.StopCapture();
        }


        public  void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var time = e.Packet.Timeval.Date;
            var len = e.Packet.Data.Length;
            try
            {
                var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
                //var tcpPacket = PacketDotNet.TcpPacket.GetEncapsulated(packet);
                var tcpPacket = (TcpPacket)packet.Extract(typeof(TcpPacket));
                if (tcpPacket != null)
                {
                    var ipPacket = (IpPacket)tcpPacket.ParentPacket;
                    System.Net.IPAddress srcIp = ipPacket.SourceAddress;
                    System.Net.IPAddress dstIp = ipPacket.DestinationAddress;
                    int srcPort = tcpPacket.SourcePort;
                    int dstPort = tcpPacket.DestinationPort;

                    SQLiteCommand insertSQL = new SQLiteCommand("INSERT INTO network (source, destination,protocol, framelength,serial,timestamp) " +
                    "VALUES (@source ,@destination,@protocol,@framelength, @serial, @timestamp)", connection);
                   
                    insertSQL.Parameters.AddWithValue("@source", Convert.ToString(srcIp + " : " + srcPort));
                    insertSQL.Parameters.AddWithValue("@destination", Convert.ToString(dstIp + " : " + dstPort));
                    insertSQL.Parameters.AddWithValue("@protocol", Convert.ToString("TCP"));
                    insertSQL.Parameters.AddWithValue("@framelength", Convert.ToString(len));
                    insertSQL.Parameters.AddWithValue("@serial", Convert.ToString(Serial++));
                    insertSQL.Parameters.AddWithValue("@timestamp", Convert.ToString(DateTime.Now));
                    insertSQL.ExecuteNonQuery();
                }
            }

            catch (Exception ex)
            {
            }
        }


        private void StopTest_Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                device.StopCapture();
                connection.Close();
                CloseButton.IsEnabled = true;
                SnackbarSeven.Background = new SolidColorBrush(Color.FromRgb(0, 255, 128));
                SnackbarSeven.MessageQueue.Enqueue("Network Connection fetch stopped successfully");
                return;

            }
            catch(Exception ex)
            {
                SnackbarSeven.Background = new SolidColorBrush(Color.FromRgb(0, 255, 128));
                SnackbarSeven.MessageQueue.Enqueue(ex.ToString());
                CloseButton.IsEnabled = true;

            }

        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
