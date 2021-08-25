using Base;
using Home.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Hotfix
{

    [ANHandler]
    public class HotfixTestHandler : IHandler
    {
        public void test(object c)
        {
            Console.WriteLine("```:4");
            var d = c as TestModel;
            d.test1();
        }
    }
    [ANService]
    public class HotfixTestService : IService
    {
        public static void test()
        {
            Console.WriteLine("aaa:4");
        }
    }

    public static class TestModelService
    {
        public static void test1(this TestModel self)
        {
            Console.WriteLine("AAA:4");
        }
    }
}
