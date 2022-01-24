using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum RoleType
    {
        NULL, //一键启动节点
        OM, //集群管理后台
        Login, //登录节点
        Home, //玩家节点
        World //世界节点
    }

    public enum GameSharedRole
    {
        Player,
        World
    }
}
