using System;
using System.Threading.Tasks;

namespace Base
{
    public abstract class AMHandler<Message> : IMHandler where Message : class
    {
        protected abstract Task Run(BaseActor session, Message message);

        public async Task Handle(BaseActor session, uint opcode, uint sn, object msg)
        {
            Message message = msg as Message;
            if (message == null)
            {
                Log.Error($"消息类型转换错误: {msg.GetType().Name} to {typeof(Message).Name}");
                return;
            }
            if (session.IsDisposed)
            {
                Log.Error($"session disconnect {msg}");
                return;
            }

            try
            {
                await this.Run(session, message);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public Type GetInType()
        {
            return typeof(Message);
        }

        public Type GetRetType()
        {
            return null;
        }
    }
}