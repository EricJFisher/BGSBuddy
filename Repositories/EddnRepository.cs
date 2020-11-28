using Interfaces.Repositories;
using Ionic.Zlib;
using NetMQ;
using NetMQ.Sockets;
using Repositories.EddnRequestTypes;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class EddnRepository : IEddnRepository
    {
        public List<EddnRequest> Submissions { get; set; } = new List<EddnRequest>();

        public async Task ListenToEddn()
        {
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

                    // ignore all outfitting and commodity responses
                    if (!result.Contains(@"""Influence"""))
                        continue;

                    Submissions.Add(EddnRequest.FromJson(result));
                }
            }
        }
    }
}
