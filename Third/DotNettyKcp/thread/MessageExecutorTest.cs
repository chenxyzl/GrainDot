using System;
using dotNetty_kcp.thread;

namespace base_kcp;

public class MessageExecutorTest : ITask
{
    private static IMessageExecutor _messageExecutor;

    public static long start = KcpUntils.currentMs();

    private static int index;

    public static int addIndex;

    public int i;

    public MessageExecutorTest(int i)
    {
        this.i = i;
    }


    public void execute()
    {
        var now = KcpUntils.currentMs();
        if (now - start > 1000)
        {
            Console.WriteLine("i " + (i - index) + "time " + (now - start));
            index = i;
            start = now;
        }
    }

    public static void en()
    {
        var i = 0;
        while (true)
        {
            var queueTest = new MessageExecutorTest(i);
            if (_messageExecutor.execute(queueTest)) i++;
        }
    }

    public static void test()
    {
        _messageExecutor = new DistuptorMessageExecutor();
        _messageExecutor.start();
        en();
    }
}