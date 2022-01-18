using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class GlobalParam
    {
        public static string SystemName = "fp";
        #region Range 扩展 in方法
        public static bool In(this Range self, int param) { return param >= self.Start.Value && param <= self.End.Value; }
        public static bool In(this Range self, uint param) { return param >= self.Start.Value && param <= self.End.Value; }
        #endregion
        /// <summary>
        /// 内部rpc消息id的范围
        /// </summary>
        public static Range InnerRpcIdRange = 1000..99999;
        /// <summary>
        /// 外部rpc消息id的范围
        /// </summary>
        public static Range OuterRpcIdRange = 100000..999999;

        /// <summary>
        /// 玩家最大分片数
        /// </summary>
        public static int MAX_NUMBER_OF_PLAYER_SHARD = 10000;
        /// <summary>
        /// 玩家最大分片数
        /// </summary>
        public static int MAX_NUMBER_OF_MAP_SHARD = 10000;
    }
}
