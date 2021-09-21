using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Common.Concurrency;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskCompletionSource = DotNetty.Common.Concurrency.TaskCompletionSource;

namespace Base.Network
{
    class WebSocketClient : BaseTcpSocketClient<IWebSocketClient, byte[]>, IWebSocketClient
    {
        public WebSocketClient(string ip, int port, string path, TcpSocketCientEvent<IWebSocketClient, byte[]> clientEvent)
            : base(ip, port, clientEvent)
        {
            string uri = $"ws://{ip}:{port}{path}";
            handshaker = WebSocketClientHandshakerFactory.NewHandshaker(
                new Uri(uri), WebSocketVersion.V13, null, true, new DefaultHttpHeaders());

            completionSource = new TaskCompletionSource();
        }
        Semaphore _handshakerSp = new Semaphore(0, 1);
        readonly WebSocketClientHandshaker handshaker;
        readonly TaskCompletionSource completionSource;
        public Task HandshakeCompletion => this.completionSource.Task;

        public override void OnChannelActive(IChannelHandlerContext ctx)
        {
            base.OnChannelActive(ctx);
            handshaker.HandshakeAsync(ctx.Channel).LinkOutcome(completionSource);
        }
        public override void OnChannelReceive(IChannelHandlerContext ctx, object msg)
        {
            PackException(() =>
            {
                IChannel ch = ctx.Channel;
                if (!handshaker.IsHandshakeComplete)
                {
                    try
                    {
                        handshaker.FinishHandshake(ch, (IFullHttpResponse)msg);
                        _handshakerSp.Release();
                        _clientEvent.OnClientStarted?.Invoke(this);
                        completionSource.TryComplete();
                    }
                    catch (WebSocketHandshakeException e)
                    {
                        Console.WriteLine("WebSocket Client failed to connect");
                        completionSource.TrySetException(e);
                    }

                    return;
                }

                if (msg is IFullHttpResponse response)
                {
                    throw new InvalidOperationException(
                        $"Unexpected FullHttpResponse (getStatus={response.Status}, content={response.Content.ToString(Encoding.UTF8)})");
                }

                if (msg is TextWebSocketFrame textFrame)
                {
                    string msgStr = textFrame.Text();
                    Console.WriteLine(msg);
                }
                else if (msg is BinaryWebSocketFrame binaryFram)
                {
                    var byets = binaryFram.Content.ToArray();
                    Console.WriteLine(byets);
                }
                else if (msg is PongWebSocketFrame)
                {
                    Console.WriteLine("WebSocket Client received pong");
                }
                else if (msg is CloseWebSocketFrame)
                {
                    Console.WriteLine("WebSocket Client received closing");
                    ch.CloseAsync();
                }
            });
        }

        public async Task Send(byte[] bytes)
        {
            try
            {
                if (!handshaker.IsHandshakeComplete)
                {
                    _handshakerSp.WaitOne();
                }
                await _channel.WriteAndFlushAsync(new BinaryWebSocketFrame(Unpooled.WrappedBuffer(bytes)));
                _clientEvent?.OnSend(this, bytes);
            }
            catch (Exception ex)
            {
                _clientEvent.OnException?.Invoke(ex);
            }
        }
    }
}
