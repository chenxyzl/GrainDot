using Base.ConfigParse;
using ExcelMapper;
using Message;

namespace Base.Config;

public class EAConfig : IExcelConfig<int>
{
    [ExcelColumnName("名称")] public string Name { get; set; }

    [ExcelColumnName("稀有度")] public Rarity Rarity { get; set; }

    [ExcelColumnName("最大堆叠数")] public uint MaxCount { get; set; }
    [ExcelColumnName("##ID")] public int Id { get; set; }
}

[SheetName("武器")]
public class EquipConfig : EAConfig
{
}

[Config("Equip")]
public class StartEquipConfigCategory : ACategory<int, EquipConfig>
{
}