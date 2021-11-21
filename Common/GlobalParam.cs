using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class GlobalParam
    {
        public static string SystemName = "iwan";
        #region Range 扩展 in方法
        public static bool In(this Range self, int param) { return param >= self.Start.Value && param <= self.End.Value; }
        public static bool In(this Range self, uint param) { return param >= self.Start.Value && param <= self.End.Value; }
        #endregion
        /// <summary>
        /// 内部rpc消息id的范围
        /// </summary>
        public static Range InnerRpcIdRange = 1000..9999;
        /// <summary>
        /// 外部rpc消息id的范围
        /// </summary>
        public static Range OuterRpcIdRange = 10000..99999;
    }
}
