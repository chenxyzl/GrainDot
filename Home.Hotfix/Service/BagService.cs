using System.Threading;
using System.Threading.Tasks;
using Home.Model.Component;
using Home.Model.State;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Share.Hotfix.Service;
using Share.Model.Component;

namespace Home.Hotfix.Service;

public static class BagService
{
    public static async Task Load(this BagComponent self)
    {
        var x = Thread.CurrentThread.ManagedThreadId.ToString();
        await self.Node.GetComponent<CallComponent>().Save(new BagState(1));
        var bagState = await self.Node.GetComponent<CallComponent>().Query<BagState>(1);
        var y = Thread.CurrentThread.ManagedThreadId.ToString();
        return;
    }
}