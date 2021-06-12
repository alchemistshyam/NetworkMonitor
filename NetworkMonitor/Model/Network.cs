using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkMonitor.Model
{
    public class Network
    {
        public int serialNumber { get; set; }
        public DateTime time { get; set; }
        public string source { get; set; }
        public string destination { get; set; }
        public string protocol { get; set; }
        public int frameLength { get; set; }

    }
}
