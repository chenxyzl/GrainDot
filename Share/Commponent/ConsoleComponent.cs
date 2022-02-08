using System.Threading;
using Base;

namespace Share.Model.Component;
public enum ConsoleMode
{
    free, //空闲状态
    repl, //交互状态
}

public class ConsoleComponent: IGlobalComponent
{
    public bool stopWatch = false;
    public ConsoleMode Mode = ConsoleMode.free;
    public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
}