using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public interface IGlobalComponent
    {
        //启动 加载db数据 ~只能处理序列化相关不要有业务逻辑
        public Task Load();
        //开始 第一个tick开始前  @param:crossDay 是否跨天
        public Task Start();
        //停止前
        public Task PreStop();
        //停止
        public Task Stop();
        //tick
        public Task Tick();
    }
}
