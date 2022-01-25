using Base.ConfigParse;
using ExcelMapper;
using Message;

namespace Base.Config;

[SheetName("英雄")]
public class HeroConfig : IExcelConfig<int>
{
    [ExcelColumnName("名称")] public string Name { get; set; }

    [ExcelColumnName("稀有度")] public Rarity Rarity { get; set; }
    [ExcelColumnName("##ID")] public int Id { get; set; }
}

[Config("Hero")]
public class HeroConfigCategory : ACategory<int, HeroConfig>
{
}