using Base.ConfigParse;
using ExcelMapper;

namespace Base.Config;

[SheetName(new[] {"材料", "道具"})]
public class ItemConfig : IExcelConfig<int>
{
    [ExcelColumnName("名称")] public string Name { get; set; } = "";

    [ExcelColumnName("最大堆叠数")] public uint MaxCount { get; set; }

    [ExcelColumnName("是用可用")] public uint used { get; set; }
    [ExcelColumnName("##ID")] public int Id { get; set; }
}

[Config("Item")]
public class ItemConfigCategory : ACategory<int, ItemConfig>
{
}

[SheetName(new[] {"产出包"})]
public class AssetsConfig : IExcelConfig<int>
{
    [ExcelColumnName("名称")] public string Name { get; set; } = "";

    [ExcelColumnName("最大堆叠数")] public uint MaxCount { get; set; }
    [ExcelColumnName("##ID")] public int Id { get; set; }
}

[Config("Item")]
public class AssetsConfigCategory : ACategory<int, AssetsConfig>
{
}