using System;
using Base;
using Base.Helper;
using Base.Network;
using Base.Serialize;
using Home.Hotfix.Service;
using Home.Model.Component;
using Message;

namespace Home.Model;

public class PlayerChannel : ICustomChannel
{
    private readonly ILog _logger;
    private string? _playerKey;

    public PlayerChannel(IBaseSocketConnection conn) : base(conn)
    {
        _logger = new NLogAdapter(conn.ConnectionId);
    }

    public override void OnConnected()
    {
        GameServer.Instance.GetComponent<ConnectionCommponent>().AddConnection(this);
    }

    public override async void OnRecieve(byte[] bytes)
    {
        if (bytes.Length == 0)
        {
            return;
        }

        Request message;
        try
        {
            message = SerializeHelper.FromBinary<Request>(bytes);
        }
        catch (Exception e) //避免协议破解
        {
            _logger.Warning(e.Message);
            Close();
            return;
        }

        var ret = new Response {Opcode = message.Opcode, Sn = message.Sn};

        //如果是ping直接回复pong
        if (message.Opcode == 200000)
        {
            ret.Code = Code.Ok;
            ret.Content = new S2CPong {Time = TimeHelper.Now()}.ToBinary();
            await _conn.Send(ret.ToBinary());
            return;
        }

        try
        {
            if (_playerKey == null)
                BindPlayerActor(message);
            else
                TellSelf(message);
        }
        catch (CodeException e) //可预料的返回客户端错误码
        {
            _logger.Warning(e.Message);

            ret.Code = e.Code;
            _ = _conn.Send(ret.ToBinary());
            if (e.Serious) Close();
        }
        catch (Exception e) //不可预料的断开客户端链接
        {
            _logger.Warning(e.Message);
            Close();
        }
    }

    public override void Close()
    {
        base.Close();
        GameServer.Instance.GetComponent<ConnectionCommponent>()
            .RemoveConnection(_conn.ConnectionId);
        //todo 通知actor下线
    }

    private void BindPlayerActor(Request message)
    {
        //第一条消息必须是登录
        A.Ensure(message.Opcode == 200003, Code.Error, "first message must login", true);
        var login = SerializeHelper.FromBinary<C2SLogin>(message.Content);
        A.Ensure(_playerKey == null, Code.Error, "player has bind", true);
        //获取actor
        //玩家没有获取到则断开链接让客户用重新走http登陆
        var player = A.NotNull(GameServer.Instance.GetComponent<LoginKeyComponent>().CheckGetKey(login.Key),
            Code.NotLogin, "player actor not found, login api may be overdue， please login again",
            true);
        //保存key
        _playerKey = login.Key;
        //填充链接id
        login.Unused = _conn.ConnectionId;
        message.Content = login.ToBinary();
        //为了高性能 只有登录消息 走Ask 其他消息都走Tell (因为需要超时)
        // var a = await player.Ask<Request>(1, TimeSpan.FromSeconds(3));
        player.Tell(message);
    }

    private void TellSelf(Request message)
    {
        _playerKey = A.NotNull(_playerKey, Code.NotLogin, "player key not set, please login again");
        var player = A.NotNull(GameServer.Instance.GetComponent<LoginKeyComponent>().CheckGetKey(_playerKey),
            Code.NotLogin, "player actor not found, may be free too long， please login again",
            true);
        player.Tell(message);
    }
}