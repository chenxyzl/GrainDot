using System;
using System.Collections.Generic;
using System.Net.Sockets;
using dotNetty_kcp.thread;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using fec;

namespace dotNetty_kcp;

public class KcpServer
{
    private readonly List<IChannel> _localAddress = new();

    private Bootstrap _bootstrap;

    private IChannelManager _channelManager;

    private IEventLoopGroup _eventLoopGroup;

    private IExecutorPool _executorPool;

    private IScheduleThread _scheduleThread;


    public void init(int workSize, KcpListener kcpListener, ChannelConfig channelConfig, params int[] ports)
    {
        _executorPool = new ExecutorPool();
        for (var i = 0; i < workSize; i++) _executorPool.CreateMessageExecutor();
        init(_executorPool, kcpListener, channelConfig, ports);
    }


    public void init(IExecutorPool executorPool, KcpListener kcpListener, ChannelConfig channelConfig,
        params int[] ports)
    {
        if (channelConfig.UseConvChannel)
        {
            var convIndex = 0;
            if (channelConfig.Crc32Check) convIndex += Ukcp.HEADER_CRC;
            if (channelConfig.FecDataShardCount != 0 && channelConfig.FecParityShardCount != 0)
                convIndex += Fec.fecHeaderSizePlus2;
            _channelManager = new ServerConvChannelManager(convIndex);
        }
        else
        {
            _channelManager = new ServerEndPointChannelManager();
        }

        var cpuNum = Environment.ProcessorCount;
        var bindTimes = cpuNum;

        _eventLoopGroup = new MultithreadEventLoopGroup(cpuNum);
        _scheduleThread = new HashedWheelScheduleThread();

        _bootstrap = new Bootstrap();
        //TODO epoll模型 服务器端怎么支持？得试试成功没有
        _bootstrap.Option(ChannelOption.SoReuseport, true);
        // _bootstrap.Option(ChannelOption.SoReuseaddr, true);
        _bootstrap.Group(_eventLoopGroup);
        _bootstrap.ChannelFactory(() => new SocketDatagramChannel(AddressFamily.InterNetwork));
        _bootstrap.Handler(new ActionChannelInitializer<SocketDatagramChannel>(channel =>
        {
            var pipeline = channel.Pipeline;
            pipeline.AddLast(new ServerChannelHandler(_channelManager, channelConfig, executorPool, kcpListener,
                _scheduleThread));
        }));

        foreach (var port in ports)
        {
//                for (int i = 0; i < bindTimes; i++) {
            var task = _bootstrap.BindAsync(port);
            var channel = task.Result;
            _localAddress.Add(channel);
//                }
        }

        //TODO 如何启动关闭进程的钩子??
    }


    /**
         * 同步关闭服务器
         */
    public void stop()
    {
        foreach (var channel in _localAddress) channel.CloseAsync().Wait();
        foreach (var ukcp in _channelManager.getAll()) ukcp.close();
        _eventLoopGroup?.ShutdownGracefullyAsync();
        _executorPool?.stop(false);
        _scheduleThread.stop();
    }
}