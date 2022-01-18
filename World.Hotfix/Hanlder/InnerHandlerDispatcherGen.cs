using Base;
using Base.Serializer;
using Home.Model;
using Message;
using System.Threading.Tasks;

namespace Home.Hotfix.Handler
{
    public partial class InnerHandlerDispatcher
    {
        public async Task<IResponse> DispatcherWithResult(PlayerActor player, InnerRequest message)
        {
            switch (message.Opcode)
            {
            }
            A.Abort(Code.Error, $"opcode:{message.Opcode} not found", true);
            return null;
        }

        public async Task DispatcherNoResult(PlayerActor player, InnerRequest message)
        {
            switch (message.Opcode)
            {
            }
            A.Abort(Code.Error, $"opcode:{message.Opcode} not found", true);
        }
    }
}
