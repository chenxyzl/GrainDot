namespace Base.Network
{
    /// <summary>
    /// TcpSocket客户端构建者
    /// </summary>
    public interface ITcpSocketClientBuilder :
        IGenericClientBuilder<ITcpSocketClientBuilder, ISocketClient, byte[]>,
        ICoderBuilder<ITcpSocketClientBuilder>
    {

    }
}
