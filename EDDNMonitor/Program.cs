using Ionic.Zlib;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.Text;

namespace EDDNMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Connecting to EDDN");
            var utf8 = new UTF8Encoding();

            using (var client = new SubscriberSocket())
            {
                client.Options.ReceiveHighWatermark = 1000;
                client.Connect("tcp://eddn.edcd.io:9500");
                client.SubscribeToAnyTopic();
                while (true)
                {
                    var bytes = client.ReceiveFrameBytes();
                    var uncompressed = ZlibStream.UncompressBuffer(bytes);
                    var result = utf8.GetString(uncompressed);
                    Console.WriteLine(result);
                }
            }
        }
    }
}
