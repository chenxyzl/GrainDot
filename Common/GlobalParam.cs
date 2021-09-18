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
        public static Range RpcIdRange = 10000..99999;
    }
}
