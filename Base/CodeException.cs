using Message;

namespace Base;

[Serializable]
public class CodeException : Exception
{
    public Code Code;
    public bool Serious; //是否是严重错误，严重的会断开客户端连接

    public CodeException(Code code, string msg, bool serious = false) : base(
        $"CodeException:: code:{code.ToString()}, msg:{msg}, serious:{serious}")
    {
        Code = code;
        Serious = serious;
    }
}