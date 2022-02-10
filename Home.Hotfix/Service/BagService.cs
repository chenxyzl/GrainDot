using System.Threading;
using System.Threading.Tasks;
using Base.Helper;
using Home.Model.Component;
using Home.Model.State;
using Share.Hotfix.Service;
using Share.Model.Component;

namespace Home.Hotfix.Service;

public static class BagService
{
    public static async Task Load(this BagComponent self)
    {
        self.State = await self.Node.GetComponent<CallComponent>().Query<BagState>(self.Node.uid);
        var y = Thread.CurrentThread.ManagedThreadId.ToString();
    }
}