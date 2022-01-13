using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public enum RpcType
    {
        Inner = 1, //内部rpc
        Outer = 2, //外部rpc
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RpcHandlerAttribute : Attribute
    {
        //public RoleDef role { get; private set; }
        public readonly uint RpcId;
        public readonly RpcType RpcType;
        public RpcHandlerAttribute(uint rpcId, RpcType rpcType)
        {
            switch (rpcType)
            {
                case RpcType.Inner:
                    {
                        A.Ensure(!GlobalParam.InnerRpcIdRange.In(rpcId), Message.Code.Error, $"inner rpc id{rpcId} range error");
                        break;
                    }
                case RpcType.Outer:
                    {
                        A.Ensure(!GlobalParam.OuterRpcIdRange.In(rpcId), Message.Code.Error, $"outer rpc id{rpcId} range error");
                        break;
                    }
                default:
                    {
                        A.Abort(Message.Code.Error, $"rpcType:{rpcType} not found");
                        break;
                    }
            }
            RpcId = rpcId;
            RpcType = rpcType;
        }
    }
}
