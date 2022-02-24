using System.Linq;
using System.Threading.Tasks;
using Base;
using Base.Helper;
using Home.Model.Component;

namespace Home.Hotfix.Service;

[Service(typeof(ConnectionComponent))]
public static class ConnectionDicService
{
    public static Task Load(this ConnectionComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Start(this ConnectionComponent self)
    {
        return Task.CompletedTask;
    }


    public static Task PreStop(this ConnectionComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Stop(this ConnectionComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Tick(this ConnectionComponent self, long now)
    {
        //tick里检查所有的链接是否有超时未登陆的 如果有则关闭链接
        while (true)
        {
            if (self.WaitAuthed.Count == 0) break;

            //因为正在登录中人数一定不多。所以这里lock写在while里。
            lock (self.LockObj)
            {
                var first = self.WaitAuthed.First();
                if (IdGenerater.ParseTime(first.Key) + 60_000 > now) break;
                //开始处理超时
                self.WaitAuthed.Remove(first.Key);
                var connection = self.GetConnection(first.Value);
                if (connection != null && !connection.authed)
                    //close 会触发删除，所以这里不用管
                    connection.Close();
            }
        }

        return Task.CompletedTask;
    }
}