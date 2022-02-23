using System;

namespace Common;

public static class GlobalParam
{
    public static string SystemName = "Z";

    /// <summary>
    ///     内部rpc消息id的范围
    /// </summary>
    public static Range InnerRpcIdRange = 1_000..99_999;

    /// <summary>
    ///     外部rpc消息id的范围
    /// </summary>
    public static Range OuterRpcIdRange = 100_000..999_999;

    /// <summary>
    ///     玩家最大分片数
    /// </summary>
    public static int MAX_NUMBER_OF_PLAYER_SHARD = 10_000;

    /// <summary>
    ///     玩家最大分片数
    /// </summary>
    public static int MAX_NUMBER_OF_WORLD_SHARD = 1_000;

    /// <summary>
    ///     rpc超时时间
    /// </summary>
    public static int RPC_TIMEOUT_TIME = 15_000;

    /// <summary>
    /// 登录和actor空闲下线时间
    /// </summary>
    public static int PLAYER_IDLE_RELEASE_TIME = 600_000_000;

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