using System.Threading.Tasks;
using Base;
using Home.Model.Component;
using Message;

namespace Home.Hotfix.Service;

[Service(typeof(MailComponent))]
public static class MailService
{
    public static Task Load(this MailComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Start(this MailComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task PreStop(this MailComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Stop(this MailComponent self)
    {
        return Task.CompletedTask;
    }

    public static Task Tick(this MailComponent self, long now)
    {
        return Task.CompletedTask;
    }

    public static S2CMails GetMails(this MailComponent self)
    {
        return new S2CMails();
    }
}