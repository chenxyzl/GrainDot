using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Base;
using Base.State;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using MongoDB.Driver;
using Share.Model.Component;

namespace Share.Hotfix.Service;

public static class ReplService
{
    public static Task Load(this ReplComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task<bool> Run(this ReplComponent self, string line, CancellationToken cancellationToken)
    {
        switch (line)
        {
            case "exit":
            {
                self.ScriptOptions = null;
                self.ScriptState = null;
                return Task.FromResult(true);
            }
            case "reset":
            {
                self.ScriptState = null;
                return Task.FromResult(false);
            }
            default:
            {
                try
                {
                    if (self.ScriptState == null)
                    {
                        self.ScriptState = await CSharpScript.RunAsync(line, self.ScriptOptions, cancellationToken: cancellationToken);
                    }
                    else
                    {
                        self.ScriptState = await self.ScriptState.ContinueWithAsync(line, cancellationToken: cancellationToken);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return Task.FromResult(false);
            }
        }
    }

    public static Task PreStop(this ReplComponent self)
    {
        self.ScriptOptions = null;
        self.ScriptState = null;
    }
}