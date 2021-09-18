using System;
using System.Threading.Tasks;
using Message;

namespace Base
{
    public abstract class AMRpcHandler<Request, Response> : IMHandler where Request : class, IMessage where Response : class, IMessage
    {
        protected abstract Task<Response> Run(BaseActor actor, Request request);

        public async Task Handle(BaseActor actor, uint opcode, uint sn, object message)
        {
            try
            {
                Request request = message as Request;
                if (request == null)
                {
                    actor.Logger.Error($"消息类型转换错误: {message.GetType().Name} to {typeof(Request).Name}");
                }
                try
                {
                    var response = await this.Run(actor, request);
                    session.Reply(sn, opcode + 1, I.PB.Code.Ok, response);
                }
                catch (CodeException e)
                {
                    Log.Error(e);
                    session.Reply(sn, opcode + 1, e.Code, null);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                    session.Reply(sn, opcode + 1, I.PB.Code.Error, null);
                }

            }
            catch (Exception e)
            {
                Log.Error($"解释消息失败: {message.GetType().FullName}\n{e}");
            }
        }

        public Type GetInType()
        {
            return typeof(Request);
        }

        public Type GetRetType()
        {
            return typeof(Response);
        }
    }
}