using Message;
using System;

namespace Base
{
    [Serializable]
    public class CodeException : Exception
    {
        public Code Code;
        public CodeException(Code code, string msg) : base($"CodeException:: code:{code.ToString()}, msg:{msg}")
        {
            this.Code = code;
        }
    }
}
