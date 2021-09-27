using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public abstract class IGlobalComponent
    {
        public IGlobalComponent(GameServer n) { Node = n; }
        protected GameServer Node;
        public K GetComponent<K>() where K : IGlobalComponent
        {
            IGlobalComponent component;
            if (!Node._components.TryGetValue(typeof(K), out component))
            {
                A.Abort(Message.Code.Error, $"game component:{typeof(K).Name} not found"); ;
            }

            return (K)component;
        }

        public void AddComponent<K>(K k) where K : IGlobalComponent
        {
            IGlobalComponent component;
            Type t = typeof(K);
            if (Node._components.TryGetValue(t, out component))
            {
                A.Abort(Message.Code.Error, $"game component:{t.Name} repeated");
            }
            var arg = new object[] { Node };
            K obj = Activator.CreateInstance(t, arg) as K;
            Node._components.Add(t, obj);
        }

        //启动 加载db数据 ~只能处理序列化相关不要有业务逻辑
        public virtual Task Load() { return Task.CompletedTask; }
        //开始 第一个tick开始前  @param:crossDay 是否跨天
        public virtual Task AfterLoad() { return Task.CompletedTask; }
        //停止前
        public virtual Task PreStop() { return Task.CompletedTask; }
        //停止
        public virtual Task Stop() { return Task.CompletedTask; }
        //每一帧 @param:now 参数为服务器当前时间
        //public abstract void Tick(long now);
    }
}
