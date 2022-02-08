using System.Threading;
using Base;
using Microsoft.CodeAnalysis.Scripting;
namespace Share.Model.Component;
public class ReplComponent: IGlobalComponent{
    public ScriptOptions? ScriptOptions;
    public ScriptState? ScriptState;
}