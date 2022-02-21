using System.Collections.Generic;
using System.Threading.Tasks;
using Base;
using Base.Helper;
using Home.Model.Component;
using Home.Model.State;
using Message;
using Share.Hotfix.Service;
using Share.Model.Component;

namespace Home.Hotfix.Service;

[Service(typeof(BagComponent))]
public static class BagService
{
    public static async Task Load(this BagComponent self)
    {
        self.State = await GameServer.Instance.GetComponent<DBComponent>().Query<BagState>(self.Node.uid, self.Node);
        //todo 初始化代码
        if (self.State == null)
        {
            var uid = IdGenerater.GenerateId();
            await GameServer.Instance.GetComponent<DBComponent>().Save(new BagState
            {
                Id = self.Node.PlayerId,
                Bag = new Dictionary<ulong, Item>
                    {{1, new Item {Uid = uid, Tid = (uint) uid, Count = 999999999, GetTime = TimeHelper.Now()}}}
            }, self.Node);
        }
    }

    public static Task Start(this BagComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task PreStop(this BagComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Stop(this BagComponent self)
    {
        return Task.CompletedTask;
    }

    public static async Task Tick(this BagComponent self, long now)
    {
        await self.CheckState();
    }

    public static async Task CheckState(this BagComponent self)
    {
        if (self.State == null) return;
        if (self.State.Dirty)
        {
            await GameServer.Instance.GetComponent<DBComponent>().Save(self.State, self.Node);
            self.State.CleanDirty();
        }
    }
}