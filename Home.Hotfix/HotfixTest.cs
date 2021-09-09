//using Akka.Actor;
//using Base;
//using Base.Helper;
//using Home.Model;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Home.Hotfix
//{

//    [ANHandler]
//    public class HotfixTestHandler : IHandler
//    {
//        public void test(object c)
//        {
//            GlobalLog.Info("```:2");
//            var d = c as TestModel;
//            d.test1();
//        }

//        void IHandler.test2(object c)
//        {
//            var d = c as TestModel;
//            int x = 1;
//            long xx = TimeHelper.NowNano();
//            for (int i = 1; i < 99999999; i++)
//            {
//                x = d.b(x);
//            }
//            xx = TimeHelper.NowNano() - xx;
//            GlobalLog.Warning("性能测试2:" + x.ToString() + "  " + xx.ToString());
//        }
//    }
//    [ANService]
//    public class HotfixTestService : IService
//    {
//        public HotfixTestService() : base(null)
//        {

//        }
//        public static void test()
//        {
//            //GlobalLog.Info("````:2");
//        }

//        public override Task Load()
//        {
//            throw new NotImplementedException();
//        }

//        public override Task OnCrossDay()
//        {
//            throw new NotImplementedException();
//        }

//        public override Task PlayerOffline(ulong playerId)
//        {
//            throw new NotImplementedException();
//        }

//        public override Task PlayerOnline(UntypedActor actor, ulong playerId)
//        {
//            throw new NotImplementedException();
//        }

//        public override Task PreStop()
//        {
//            throw new NotImplementedException();
//        }

//        public override Task Start(bool crossDay)
//        {
//            throw new NotImplementedException();
//        }

//        public override Task Stop()
//        {
//            throw new NotImplementedException();
//        }

//        public override void Tick(long now)
//        {
//            throw new NotImplementedException();
//        }
//    }

//    public static class TestModelService
//    {
//        public static void test1(this TestModel self)
//        {
//            GlobalLog.Info("````:2");
//        }

//        public static int b(this TestModel self, int i)
//        {
//            return i + 1;
//        }
//    }
//}
