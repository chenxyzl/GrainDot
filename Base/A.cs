using Message;

namespace Base
{
    public static class A
    {
        public static void Ensure(bool a, Code code, string des = null)
        {
            if (a != true)
            {
                throw new CodeException(code, des ?? code.ToString());
            }
        }

        public static void Abort(Code code, string des = null)
        {
            throw new CodeException(code, des ?? code.ToString());
        }

        public static T RequireNotNull<T>(T t, Code code, string des = null)
        {
            if (t == null)
            {
                throw new CodeException(code, des ?? code.ToString());
            }
            return t;
        }
    }
}
