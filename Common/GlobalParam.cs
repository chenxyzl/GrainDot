using System;

namespace Common;

public static class GlobalParam
{
    public static string SystemName = "Z";

    /// <summary>
    ///     内部rpc消息id的范围
    /// </summary>
    public static Range InnerRpcIdRange = 1000..99999;

    /// <summary>
    ///     外部rpc消息id的范围
    /// </summary>
    public static Range OuterRpcIdRange = 100000..999999;

    /// <summary>
    ///     玩家最大分片数
    /// </summary>
    public static int MAX_NUMBER_OF_PLAYER_SHARD = 10000;

    /// <summary>
    ///     玩家最大分片数
    /// </summary>
    public static int MAX_NUMBER_OF_WORLD_SHARD = 1000;

    /// <summary>
    ///     rpc超时时间
    /// </summary>
    public static int RPC_TIMEOUT_TIME = 15000;

    #region Range 扩展 in方法

    public static bool In(this Range self, int param)
    {
        return param >= self.Start.Value && param <= self.End.Value;
    }

    public static bool In(this Range self, uint param)
    {
        return param >= self.Start.Value && param <= self.End.Value;
    }

    #endregion
}