using System.Threading.Tasks;
using Common;
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
        if (self.State.Version == DBVersion.Null)
        {
            //todo 初始化代码
        }
    }
}