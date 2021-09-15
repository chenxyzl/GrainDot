using Base.Network.Client;
using Base.Network.Shared;
using Base.Network.Shared.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Robot
{
    class Program
    {
        static async Task Main(string[] args)
        {

            Thread.Sleep(10_000);
            await Test.Instance.StartAsync();
        }


    }

    class Test : Single<Test>
    {
        public async Task StartAsync()
        {
            var client = new Client();
            client.MessageReceivedHandler = OnMessageReceived;
            client.Connect("127.0.0.1", 7700);


            var cancellationTokenSource = new CancellationTokenSource();
            while (true)
            {
                try
                {
                    string line = await Task.Factory.StartNew(() =>
                    {
                        return Console.In.ReadLine();
                    }, cancellationTokenSource.Token);

                    if (line == null)
                    {
                        break;
                    }

                    line = line.Trim();

                    switch (line)
                    {
                        case "quit":
                            {
                                break;
                            }
                        default:
                            {
                                /// send a message to server
                                using (var writer = BufferWriter.Create())
                                {
                                    writer.Write("Test message!");
                                    client.SendMessage(writer);
                                }
                                break;
                            }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            // disconnect from the server when we are done
            client.Disconnect();
        }

        // implements the callback for MessageReceivedHandler
        private void OnMessageReceived(IBufferReader reader)
        {
            Console.WriteLine($"Received data from server, data length {reader.Length}");
        }
    }
}
