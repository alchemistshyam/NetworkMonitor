using MahApps.Metro.Controls;
using NetworkMonitor.Model;
using SharpPcap;
using PacketDotNet;
using SharpPcap.AirPcap;
using SharpPcap.WinPcap;
using SharpPcap.LibPcap;
using System.Collections.ObjectModel;
using System;

namespace NetworkMonitor.Utilities
{
    class NetrworkDevice
    {
        public ObservableCollection<string> CaptureDevice()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();

            var devices = LibPcapLiveDeviceList.Instance;

            if (devices.Count < 1)
            {
                return list;
            }

            int i = 0;

            foreach (var dev in devices)
            {
                list.Add(Convert.ToString(dev.Interface.FriendlyName));
                //Console.WriteLine("{0}) {1} {2}", i, dev.Name, dev.Interface.FriendlyName);
                i++;
            }
            return list;
        }
    }
}
