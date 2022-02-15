using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DotNetty.Transport.Channels;
using Message;

namespace Base.Network;

internal abstract class BaseTcpSocketServer<TSocketServer, TConnection, TData> : IBaseTcpSocketServer<TConnection>,
    IChannelEvent
    where TConnection : class, IBaseSocketConnection
    where TSocketServer : class, IBaseTcpSocketServer<TConnection>
{
    public BaseTcpSocketServer(int port, TcpSocketServerEvent<TSocketServer, TConnection, TData> eventHandle)
    {
        Port = port;
        _eventHandle = eventHandle;
    }

    #region 私有成员

    private ConcurrentDictionary<string, TConnection> _idMapConnections { get; } = new();

    protected IChannel _serverChannel { get; set; } = null!;

    protected void AddConnection(TConnection theConnection)
    {
        if (_idMapConnections.TryRemove(theConnection.ConnectionId, out var conn))
        {
            A.Abort(Code.Error, $"connectionId {theConnection.ConnectionId} repeated");
            conn.Close();
        }

        _idMapConnections[theConnection.ConnectionId] = theConnection;
    }

    protected TConnection GetConnection(IChannel clientChannel)
    {
        var id = clientChannel.Id.AsShortText();
        _idMapConnections.TryGetValue(clientChannel.Id.AsShortText(), out var conn);
        return A.NotNull(conn, Code.Error, $"conn:{id} not found");
    }

    protected void PackException(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            _eventHandle.OnException?.Invoke(ex);
        }
    }

    protected TcpSocketServerEvent<TSocketServer, TConnection, TData> _eventHandle { get; }
    protected abstract TConnection BuildConnection(IChannel clientChannel);

    #endregion

    #region 外部接口

    public void OnChannelActive(IChannelHandlerContext ctx)
    {
        var theConnection = BuildConnection(ctx.Channel);
        AddConnection(theConnection);
        _eventHandle.OnNewConnection?.Invoke(A.NotNull(this as TSocketServer), theConnection);
        theConnection.OnConnected();
    }

    public void OnException(IChannel clientChannel, Exception ex)
    {
        PackException(() =>
        {
            var theConnection = GetConnection(clientChannel);
            CloseConnection(theConnection);
            _eventHandle.OnException?.Invoke(ex);
        });
    }

    public void OnChannelInactive(IChannel clientChannel)
    {
        PackException(() =>
        {
            var theConnection = GetConnection(clientChannel);
            RemoveConnection(theConnection);
            _eventHandle.OnConnectionClose?.Invoke(A.NotNull(this as TSocketServer), theConnection);
            theConnection.OnClose();
        });
    }

    public void SetChannel(IChannel channel)
    {
        _serverChannel = channel;
    }

    public int Port { get; }

    public List<TConnection> GetAllConnections()
    {
        return _idMapConnections.Values.Select(x => x).ToList();
    }

    public TConnection GetConnectionById(string connectionId)
    {
        _idMapConnections.TryGetValue(connectionId, out var conn);
        return A.NotNull(conn, Code.Error, $"conn:{connectionId} not found");
    }

    public int GetConnectionCount()
    {
        return _idMapConnections.Count;
    }

    public void RemoveConnection(TConnection theConnection)
    {
        _idMapConnections.TryRemove(theConnection.ConnectionId, out _);
    }

    public void CloseConnection(TConnection theConnection)
    {
        theConnection.Close();
    }

    public void Close()
    {
        _serverChannel.CloseAsync();
    }

    public abstract void OnChannelReceive(IChannelHandlerContext ctx, object msg);

    #endregion
}