using System;

namespace Proto
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (CommandHelper.Instance.Parse(args))
                return;

            // Inner.proto生成cs代码
            InnerProto2CS.Proto2CS();
            if (CommandHelper.Instance.GenServer) InnerProto2CSn.Proto2CS();

            if (CommandHelper.Instance.GenClient) GenClient.Proto2CS();

            Console.WriteLine("proto2cs succeed!");
        }
    }
}