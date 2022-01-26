using Message;

namespace Base.Helper;

public class IdGenerater
{
    private static readonly int _nodeFlag = 12; //可表示4096个服务器节点
    private static readonly ulong _nodeLimit = 1UL << 12;
    private static readonly int _timeFlag = 42; //可表示从1970.1.1:0:0:0的139年(可以改成从任意时间点开始算)
    private static readonly ulong _timeFlagLimit = 1UL << 42;
    private static readonly int _incFlag = 10; //每毫秒2^10个id
    private static readonly ulong _incFlagLimit = 1UL << 10;
    private readonly uint _node;
    private ulong _lastTime;
    private uint _value;

    public IdGenerater(uint node)
    {
        if (node >= _nodeLimit) A.Abort(Code.Error, $"node Id must less than {_nodeLimit}", true);

        _node = node;
    }


    public static Tuple<uint, ulong, uint> Parase(ulong id)
    {
        var node = id >> (_timeFlag + _incFlag);
        var time = (id << _nodeFlag) >> (_nodeFlag + _incFlag);
        var value = (id << (_nodeFlag + _timeFlag)) >> (_nodeFlag + _timeFlag);
        return new Tuple<uint, ulong, uint>((uint) node, time, (uint) value);
    }


    public ulong GenerateId()
    {
        while (true)
        {
            var time = (ulong) TimeHelper.Now();
            if (time > _lastTime)
            {
                _value = 0;
                _lastTime = time;
            }

            //如果超过了则服务器等待到下一毫秒再生成
            if (++_value >= _incFlagLimit) continue;

            if (time >= _timeFlagLimit) A.Abort(Code.Error, $"time >= {_timeFlagLimit} value: {time}", true);

            return toULong(_node, time, _value);
        }
    }

    public ulong toULong(uint node, ulong time, uint value)
    {
        ulong result;
        result = value;
        result |= time << _incFlag;
        result |= (ulong) node << (_timeFlag + _incFlag);
        return result;
    }
}