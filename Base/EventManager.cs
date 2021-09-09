using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public class EventManager
    {
        #region 单例
        private EventManager() { }
        private static readonly EventManager singleInstance = new EventManager();
        public static EventManager Instance { get { return singleInstance; } }
        #endregion
        public void AddEvent()
        {

        }
        public void RemoveEvent()
        {

        }
        public void Tick()
        {
            //检查时间超时

        }
    }
}
