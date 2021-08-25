using PB;
using System;

namespace Base
{
    [Serializable]
    public class CodeException : Exception
    {
        public Code Code;
        public string Msg;
        public CodeException(Code code, string msg) : base($"CodeException:: code:{code.ToString()}, msg:{msg}")
        {
            this.Code = code;
            this.Msg = msg;
        }
    }
}
