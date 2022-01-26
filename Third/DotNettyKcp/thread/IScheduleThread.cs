using System;

namespace dotNetty_kcp.thread;

public interface IScheduleThread
{
    void schedule(IScheduleTask scheduleTask, TimeSpan timeSpan);


    void stop();
}