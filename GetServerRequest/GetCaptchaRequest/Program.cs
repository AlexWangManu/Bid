using System;
using System.Collections.Generic;
using SharpPcap;
using System.IO;
using System.Drawing;
using SharpPcap.LibPcap;
using System.Text;
using System.Net;
using System.Configuration;

namespace GetCaptchaRequest
{
    /// <summary>
    /// Basic capture example with no callback
    /// </summary>
    public class Program
    {
        static Dictionary<string, bool> historyMessage = new Dictionary<string, bool>();
        static int instanceIndex = int.Parse(ConfigurationManager.AppSettings["InstanceIndex"]);
        static string serverIpAddress = ConfigurationManager.AppSettings["ServerIpAddress"];
        static string imageSavePath = ConfigurationManager.AppSettings["ImageSavePath"];
        
        public static void Main(string[] args)
        {
            var devices = CaptureDeviceList.Instance;
            var device = devices[instanceIndex];
            device.OnPacketArrival += new PacketArrivalEventHandler(device_OnPacketArrival);
            device.Open();
            device.Filter = string.Format("tcp port 80 && dst {0}", serverIpAddress);
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
                    string startStr = "GET";
                    string endStr = "HTTP";
                    int startIndex = requestPayLoad.IndexOf(startStr) + startStr.Length;
                    int endIndex = requestPayLoad.IndexOf(endStr);
                    string url = string.Format(@"http://{0}{1}", serverIpAddress, requestPayLoad.Substring(startIndex, endIndex - startIndex).Trim());
                    if (!historyMessage.ContainsKey(url))
                    {
                        var request = WebRequest.Create(url);
                        var response = request.GetResponse();
                        using (var stream = response.GetResponseStream())
                        {
                            var image = Image.FromStream(stream);
                            image.Save(imageSavePath);
                            Console.WriteLine(DateTime.Now + " : " + imageSavePath);
                        }
                        historyMessage.Add(url, true);
                    }
                    else // If contains, just remove it, so that it will not make historyMessage too huge
                    {
                        historyMessage.Remove(url);
                    }
                }
            }
        }
    }
}
