using System.Collections.Generic;
using System.Threading.Tasks;
using Base.Helper;
using Home.Model.Component;
using Home.Model.State;
using Message;
using Share.Hotfix.Service;
using Share.Model.Component;

namespace Home.Hotfix.Service;

public static class BagService
{
    public static async Task Load(this BagComponent self)
    {
        self.State = await self.Node.GetComponent<DBComponent>().Query<BagState>(self.Node.uid, self.Node);
        //todo 初始化代码
        if (self.State == null)
        {
            var uid = IdGenerater.GenerateId();
            await self.Node.GetComponent<DBComponent>().Save(new BagState
            {
                Id = self.Node.PlayerId,
                Bag = new Dictionary<ulong, Item>
                    {{1, new Item {Uid = uid, Tid = (uint) uid, Count = 999999999, GetTime = TimeHelper.Now()}}}
            }, self.Node);
        }
    }
}