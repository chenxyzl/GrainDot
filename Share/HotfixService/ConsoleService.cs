using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Base;
using Base.State;
using MongoDB.Driver;
using Share.Model.Component;

namespace Share.Hotfix.Service;

public static class ConsoleService
{
    public static async Task WaitingRead(this ConsoleComponent self)
    {
        do
        {
            try
            {
                string line = await Task.Factory.StartNew(() => Console.In.ReadLine(),
                    self.CancellationTokenSource.Token);

                if (line == null)
                {
                    break;
                }

                line = line.Trim();

                switch (self.Mode)
                {
                    case ConsoleMode.free:
                        break;
                    case ConsoleMode.repl:
                        var isExited = true;
                        ReplComponent replComponent = GameServer.Instance.GetComponent<ReplComponent>();
                        if (replComponent == null)
                        {
                            Console.WriteLine($"no command: {line}!");
                            break;
                        }
                        try
                        {
                            isExited = await replComponent.Run(line, self.CancellationTokenSource.Token);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        if (isExited)
                        {
                            self.Mode = ConsoleMode.free;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                if (self.Mode != "")
                {
                    bool isExited = true;
                    switch (this.Mode)
                    {
                        case ConsoleMode.Repl:
                        {
                            ReplComponent replComponent = this.GetComponent<ReplComponent>();
                            if (replComponent == null)
                            {
                                Console.WriteLine($"no command: {line}!");
                                break;
                            }

                            try
                            {
                                isExited = await replComponent.Run(line, this.CancellationTokenSource.Token);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }

                            break;
                        }
                    }

                    if (isExited)
                    {
                        this.Mode = "";
                    }

                    continue;
                }

                switch (line)
                {
                    case "reload":
                        try
                        {
                            await HotfixManager.Instance.Reload();
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
                            await ConfigManager.Instance.ReloadConfig();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    case "repl":
                        try
                        {
                            this.Mode = ConsoleMode.Repl;
                            await this.AddComponent<ReplComponent>();
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        } while (false);
        await WaitingRead(self);
    }

    public static Task DoFreeCommand(this ConsoleComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Load(this ConsoleComponent self)
    {
        _ = WaitingRead(self);
        return Task.CompletedTask;
    }

    public static Task PreStop(this ConsoleComponent self)
    {
        self.CancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }
}