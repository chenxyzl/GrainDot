
using Base;

namespace Share.Model
{
    public partial class TestTxtConfig : IConfig<ulong>
    {
        public ulong Id { get; set; }
        public string OuterIP;
    }

    [Config("TextConfig")]
    public partial class StartMachineConfigCategory : ACategory<ulong, TestTxtConfig> { }

}
