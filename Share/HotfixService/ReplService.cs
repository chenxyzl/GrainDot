using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Share.Model.Component;

namespace Share.Hotfix.Service;

public static class ReplService
{
    public static Task Load(this ReplComponent self)
    {
        return Task.CompletedTask;
    }

    public static void EnterRepl(this ReplComponent self)
    {
        self.ScriptOptions = ScriptOptions.Default
            .WithMetadataResolver(ScriptMetadataResolver.Default.WithBaseDirectory(Environment.CurrentDirectory))
            .AddReferences(typeof(ReplComponent).Assembly)
            .AddImports("System");
    }

    //todo 注意 这里需要切换到global的主线程
    public static async Task<bool> Run(this ReplComponent self, string line, CancellationToken cancellationToken)
    {
        switch (line)
        {
            case "exit":
            {
                self.ScriptOptions = null;
                self.ScriptState = null;
                return true;
            }
            case "reset":
            {
                self.ScriptState = null;
                return false;
            }
            default:
            {
                try
                {
                    if (self.ScriptState == null)
                        self.ScriptState = await CSharpScript.RunAsync(line, self.ScriptOptions,
                            cancellationToken: cancellationToken);
                    else
                        self.ScriptState =
                            await self.ScriptState.ContinueWithAsync(line, cancellationToken: cancellationToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return false;
            }
        }
    }

    public static void PreStop(this ReplComponent self)
    {
        self.ScriptOptions = null;
        self.ScriptState = null;
    }
}