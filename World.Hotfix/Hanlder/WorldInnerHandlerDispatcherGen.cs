using Base;
using Base.Serializer;
using World.Model;
using Message;
using System.Threading.Tasks;

namespace Home.Hotfix.Handler
{
    public partial class WorldInnerHandlerDispatcher
    {
        public async Task<IResponse> DispatcherWithResult(WorldSession player, InnerRequest message)
        {
            switch (message.Opcode)
            {
            }
            A.Abort(Code.Error, $"opcode:{message.Opcode} not found", true);
            return null;
        }

        public async Task DispatcherNoResult(WorldSession player, InnerRequest message)
        {
            switch (message.Opcode)
            {
            }
            A.Abort(Code.Error, $"opcode:{message.Opcode} not found", true);
        }
    }
}
