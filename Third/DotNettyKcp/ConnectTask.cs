using System;
using dotNetty_kcp.thread;

namespace dotNetty_kcp;

public class ConnectTask : ITask
{
    private readonly KcpListener _listener;
    private readonly Ukcp _ukcp;

    public ConnectTask(Ukcp ukcp, KcpListener listener)
    {
        _ukcp = ukcp;
        _listener = listener;
    }


    public void execute()
    {
        try
        {
            _listener.onConnected(_ukcp);
        }
        catch (Exception e)
        {
            _listener.handleException(e, _ukcp);
        }
    }
}