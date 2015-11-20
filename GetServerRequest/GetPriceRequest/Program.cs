using System;
using System.Collections.Generic;
using SharpPcap;
using System.IO;
using System.Drawing;
using SharpPcap.LibPcap;
using System.Text;
using System.Net;
using System.Configuration;

namespace GetLatestPriceRequest
{
    /// <summary>
    /// Basic capture example with no callback
    /// </summary>
    public class Program
    {
        static Dictionary<string, bool> historyMessage = new Dictionary<string, bool>();
        static int instanceIndex = int.Parse(ConfigurationManager.AppSettings["InstanceIndex"]);
        static string serverIpAddress = ConfigurationManager.AppSettings["ServerIpAddress"];
        static string priceSavePath = ConfigurationManager.AppSettings["PriceSavePath"];

        public static void Main(string[] args)
        {
            var devices = CaptureDeviceList.Instance;
            var device = devices[instanceIndex];
            //device = new CaptureFileReaderDevice(@"D:\TDDOWNLOAD\secondfullbid.s0i0.pcap");            
            
            // Register our handler function to the 'packet arrival' event
            device.OnPacketArrival +=
                new PacketArrivalEventHandler(device_OnPacketArrival);
            
            
            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            device.Open();
            device.Filter = string.Format("tcp && src {0}", serverIpAddress);
            device.Capture();
            device.Close();
        }

        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var packet = PacketDotNet.Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            var tcpPacket = PacketDotNet.TcpPacket.GetEncapsulated(packet);
            if (tcpPacket != null)
            {
                if (tcpPacket.PayloadData.GetLength(0) >= 1)
                {
                    
                    var requestPayLoad = Encoding.UTF8.GetString(tcpPacket.PayloadData);
                    string endStr = string.Format(",{0:yyyy-MM-dd}", DateTime.Now.Date);
                    //string endStr = string.Format(",{0:yyyy-MM-dd}", new DateTime(2015, 7, 18));
                    int endIndex = requestPayLoad.IndexOf(endStr);
                    if (endIndex >= 0)
                   {
                        string potentialPriceString = requestPayLoad.Substring(0, endIndex);
                        string startStr = ",";
                        int startIndex = potentialPriceString.LastIndexOf(startStr);
                        string price = potentialPriceString.Substring(startIndex + 1);
                        WritePrice(price);
                        Console.WriteLine(DateTime.Now.ToString() + ": " + price);
                    }
                }
            }
        }

        private static void WritePrice(string text)
        {
            using(FileStream fs = new FileStream(priceSavePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                {
                    sw.Write(text);
                }
            }
        }
    }
}
