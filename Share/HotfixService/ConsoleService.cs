using System;
using System.Threading.Tasks;
using Base;
using Share.Model.Component;

namespace Share.Hotfix.Service;

[Service(typeof(ConsoleComponent))]
public static class ConsoleService
{
    public static async Task WaitingRead(this ConsoleComponent self)
    {
        do
        {
            try
            {
                var line = await Task.Factory.StartNew(() => Console.In.ReadLine(),
                    self.CancellationTokenSource.Token);

                if (line == null) break;

                line = line.Trim();

                switch (self.Mode)
                {
                    case ConsoleMode.free:
                        await DoFreeCommand(self, line);
                        break;
                    case ConsoleMode.repl:
                        var isExited = true;
                        try
                        {
                            var replComponent = GameServer.Instance.GetComponent<ReplComponent>();
                            isExited = await replComponent.Run(line, self.CancellationTokenSource.Token);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        if (isExited) self.Mode = ConsoleMode.free;

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        } while (!self.stopWatch);

        //读取下一条命令
        await WaitingRead(self);
    }

    public static Task DoFreeCommand(this ConsoleComponent self, string line)
    {
        switch (line)
        {
            case "reload":
                try
                {
                    HotfixManager.Instance.Reload();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                break;
            case "table":
                try
                {
                    //重新加载配置
                    ConfigManager.Instance.ReloadConfig();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                break;
            case "repl":
                try
                {
                    self.Mode = ConsoleMode.repl;
                    GameServer.Instance.GetComponent<ReplComponent>().EnterRepl();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                break;
            default:
                Console.WriteLine($"no such command: {line}");
                break;
        }

        return Task.FromResult(true);
    }

    public static Task Load(this ConsoleComponent self)
    {
        _ = WaitingRead(self);
        return Task.CompletedTask;
    }

    public static Task PreStop(this ConsoleComponent self)
    {
        self.stopWatch = true;
        self.CancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }

    public static Task Start(this ConsoleComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Stop(this ConsoleComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Tick(this ConsoleComponent self, long dt)
    {
        return Task.CompletedTask;
    }
}