using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    public interface IHandler
    {
        void test(object c);

        void test2(object c);


        public int a(int i)
        {
            return i + 1;
        }
    }
}
