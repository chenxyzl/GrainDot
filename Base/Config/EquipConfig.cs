
using ExcelMapper;
using Base.ConfigParse;
using Message;

namespace Base.Config
{

    public class EAConfig : IExcelConfig<int>
    {
        [ExcelColumnName("##ID")]
        public int Id { get; set; }

        [ExcelColumnName("名称")]
        public string Name { get; set; }

        [ExcelColumnName("稀有度")]
        public Rarity Rarity { get; set; }

        [ExcelColumnName("最大堆叠数")]
        public uint MaxCount { get; set; }
    }

    [SheetName("武器")]
    public class EquipConfig : EAConfig
    {
    }

    [Config("Equip")]
    public partial class StartEquipConfigCategory : ACategory<int, EquipConfig> { }
}
