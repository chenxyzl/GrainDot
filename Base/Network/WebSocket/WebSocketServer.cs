using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs.Http;
using DotNetty.Codecs.Http.WebSockets;
using DotNetty.Transport.Channels;

namespace Base.Network;

internal class WebSocketServer<T> : BaseTcpSocketServer<IWebSocketServer, IWebSocketConnection, byte[]>,
    IWebSocketServer
    where T : WebSocketConnection
{
    private WebSocketServerHandshaker handshaker;

    public WebSocketServer(int port, string path,
        TcpSocketServerEvent<IWebSocketServer, IWebSocketConnection, byte[]> eventHandle)
        : base(port, eventHandle)
    {
        _path = path;
    }

    protected string _path { get; }

    protected override IWebSocketConnection BuildConnection(IChannel clientChannel)
    {
        var arg = new object[] {this, clientChannel, _eventHandle};
        return Activator.CreateInstance(typeof(T), arg) as T;
    }

    public override void OnChannelReceive(IChannelHandlerContext ctx, object msg)
    {
        if (msg is IFullHttpRequest request)
            HandleHttpRequest(ctx, request);
        else if (msg is WebSocketFrame frame) HandleWebSocketFrame(ctx, frame);
    }

    private void HandleHttpRequest(IChannelHandlerContext ctx, IFullHttpRequest req)
    {
        // Handle a bad request.
        if (!req.Result.IsSuccess)
        {
            SendHttpResponse(ctx, req,
                new DefaultFullHttpResponse(HttpVersion.Http11, HttpResponseStatus.BadRequest));
            return;
        }

        // Allow only GET methods.
        if (req.Method.ToString().ToUpper() != "GET")
        {
            SendHttpResponse(ctx, req,
                new DefaultFullHttpResponse(HttpVersion.Http11, HttpResponseStatus.Forbidden));
            return;
        }

        //Wesocket握手协议
        if (req.Uri == _path)
        {
            var wsFactory = new WebSocketServerHandshakerFactory(
                GetWebSocketLocation(req), null, true, 5 * 1024 * 1024);
            handshaker = wsFactory.NewHandshaker(req);
            if (handshaker == null)
                WebSocketServerHandshakerFactory.SendUnsupportedVersionResponse(ctx.Channel);
            else
                handshaker.HandshakeAsync(ctx.Channel, req);
        }
    }

    private void HandleWebSocketFrame(IChannelHandlerContext ctx, WebSocketFrame frame)
    {
        // Check for closing frame
        if (frame is CloseWebSocketFrame)
        {
            handshaker.CloseAsync(ctx.Channel, (CloseWebSocketFrame) frame.Retain());
            return;
        }

        if (frame is PingWebSocketFrame)
        {
            ctx.WriteAsync(new PongWebSocketFrame((IByteBuffer) frame.Content.Retain()));
            return;
        }

        if (frame is TextWebSocketFrame textIframe)
        {
            ctx.WriteAsync(new TextWebSocketFrame("not support text!!! please use binary!!!"));
            return;
        }

        if (frame is BinaryWebSocketFrame binaryFram)
            PackException(() =>
            {
                var byets = binaryFram.Content.ToArray();
                var theConnection = GetConnection(ctx.Channel);
                _eventHandle.OnRecieve?.Invoke(this, theConnection, byets);
                theConnection.OnRecieve(byets);
            });
    }

    private static void SendHttpResponse(IChannelHandlerContext ctx, IFullHttpRequest req, IFullHttpResponse res)
    {
        // Generate an error page if response getStatus code is not OK (200).
        if (res.Status.Code != 200)
        {
            var buf = Unpooled.CopiedBuffer(Encoding.UTF8.GetBytes(res.Status.ToString()));
            res.Content.WriteBytes(buf);
            buf.Release();
            HttpUtil.SetContentLength(res, res.Content.ReadableBytes);
        }

        // Send the response and close the connection if necessary.
        var task = ctx.Channel.WriteAndFlushAsync(res);
        if (!HttpUtil.IsKeepAlive(req) || res.Status.Code != 200)
            task.ContinueWith((t, c) => ((IChannelHandlerContext) c).CloseAsync(),
                ctx, TaskContinuationOptions.ExecuteSynchronously);
    }

    private string GetWebSocketLocation(IFullHttpRequest req)
    {
        _ = req.Headers.TryGet(HttpHeaderNames.Host, out var value);
        var location = value.ToString() + _path;
        return "ws://" + location;
    }
}