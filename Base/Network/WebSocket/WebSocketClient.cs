using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using TaskCompletionSource = DotNetty.Common.Concurrency.TaskCompletionSource;

namespace Base.Network;

internal class WebSocketClient : BaseSocketClient<ISocketClient, byte[]>, ISocketClient
{
    private readonly Semaphore _handshakerSp = new(0, 1);
    private readonly TaskCompletionSource completionSource;
    private readonly WebSocketClientHandshaker handshaker;

    public WebSocketClient(IPAddress ip, int port, string path, TcpSocketCientEvent<ISocketClient, byte[]> clientEvent)
        : base(ip, port, clientEvent)
    {
        var uri = $"ws://{ip}:{port}{path}";
        handshaker = WebSocketClientHandshakerFactory.NewHandshaker(
            new Uri(uri), WebSocketVersion.V13, null, true, new DefaultHttpHeaders());

        completionSource = new TaskCompletionSource();
    }

    public Task HandshakeCompletion => completionSource.Task;

    public override async Task Send(byte[] bytes)
    {
        try
        {
            if (!handshaker.IsHandshakeComplete) _handshakerSp.WaitOne();

            await _channel.WriteAndFlushAsync(new BinaryWebSocketFrame(Unpooled.WrappedBuffer(bytes)));
            _clientEvent.OnSend?.Invoke(this, bytes);
        }
        catch (Exception ex)
        {
            _clientEvent.OnException?.Invoke(ex);
        }
    }

    public override async Task Send(string msgStr)
    {
        try
        {
            if (!handshaker.IsHandshakeComplete) _handshakerSp.WaitOne();

            await _channel.WriteAndFlushAsync(new TextWebSocketFrame(msgStr));
            //_clientEvent?.OnSend(this, msgStr);
        }
        catch (Exception ex)
        {
            _clientEvent.OnException?.Invoke(ex);
        }
    }

    public override void OnChannelActive(IChannelHandlerContext ctx)
    {
        base.OnChannelActive(ctx);
        handshaker.HandshakeAsync(ctx.Channel).LinkOutcome(completionSource);
    }

    public override void OnChannelReceive(IChannelHandlerContext ctx, object msg)
    {
        PackException(() =>
        {
            var ch = ctx.Channel;
            if (!handshaker.IsHandshakeComplete)
            {
                try
                {
                    handshaker.FinishHandshake(ch, (IFullHttpResponse) msg);
                    _handshakerSp.Release();
                    _clientEvent.OnClientStarted?.Invoke(this);
                    completionSource.TryComplete();
                }
                catch (WebSocketHandshakeException e)
                {
                    GlobalLog.Warning("WebSocket Client failed to connect");
                    completionSource.TrySetException(e);
                }

                return;
            }

            if (msg is IFullHttpResponse response)
                throw new InvalidOperationException(
                    $"Unexpected FullHttpResponse (getStatus={response.Status}, content={response.Content.ToString(Encoding.UTF8)})");

            if (msg is TextWebSocketFrame textFrame)
            {
                var msgStr = textFrame.Text();
                Console.WriteLine(msg);
            }
            else if (msg is BinaryWebSocketFrame binaryFram)
            {
                var byets = binaryFram.Content.ToArray();
                Console.WriteLine(byets);
            }
            else if (msg is PongWebSocketFrame)
            {
                // Console.WriteLine("WebSocket Client received pong");
            }
            else if (msg is CloseWebSocketFrame)
            {
                // GlobalLog.Warning("WebSocket Client received closing");
                ch.CloseAsync();
            }
        });
    }
}