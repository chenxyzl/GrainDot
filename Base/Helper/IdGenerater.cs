using Message;
using System;
using System.Runtime.InteropServices;

namespace Base.Helper
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IdStruct
    {
        public uint Time; // 32bit
        public ushort Value; // 16bit
        public ushort Node; // 16bit

        public ulong ToULong()
        {
            ulong result = 0;
            result |= (uint)this.Node;
            result |= (ulong)this.Value << 16;
            result |= (ulong)this.Time << 32;
            return result;
        }

        public IdStruct(ushort node, uint time, ushort value)
        {
            this.Node = node;
            this.Time = time;
            this.Value = value;
        }

        public IdStruct(ulong id)
        {
            ulong result = (ulong)id;
            this.Node = (ushort)(result & (ushort.MaxValue));
            result >>= 16;
            this.Value = (ushort)(result & (byte.MaxValue));
            result >>= 16;
            this.Time = (uint)result;
        }

        public override string ToString()
        {
            return $"node: {this.Node}, time: {this.Time}, value: {this.Value}";
        }
    }

    public static class IdGenerater
    {
        private static uint value;

        public static ushort Node //18位
        {
            set;
            get;
        }


        public static ushort GetNode(ulong v)
        {
            return new IdStruct(v).Node;
        }

        public static long lastTime;

        public static ulong GenerateId()
        {
            long time = TimeHelper.NowSeconds();
            if (time != lastTime)
            {
                value = 0;
                lastTime = time;
            }




            if (++value > ushort.MaxValue - 1)
            {
                A.Abort(Code.Error, $"id is not enough! value: {value}");
            }

            if (time > int.MaxValue)
            {
                A.Abort(Code.Error, $"time > int.MaxValue value: {time}");
            }

            IdStruct idStruct = new IdStruct(Node, (uint)time, (ushort)value);
            return idStruct.ToULong();
        }
    }
}