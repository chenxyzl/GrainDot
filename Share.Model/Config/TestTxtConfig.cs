
using Base;

namespace Share.Model
{
    [Config("TextConfig")]
    public partial class StartMachineConfigCategory : ACategory<ulong, TestTxtConfig>
    {
        public static StartMachineConfigCategory Instance;
        public StartMachineConfigCategory()
        {
            Instance = this;
        }
    }

    public partial class TestTxtConfig : IConfig<ulong>
    {
        public ulong Id { get; set; }
        public string OuterIP;
    }
}
