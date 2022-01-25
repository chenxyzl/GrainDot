using Base.ConfigParse;
using ExcelMapper;
using Message;

namespace Base.Config;

[SheetName("货币")]
public class CurrencyConfig : IExcelConfig<CurrencyType>
{
    [ExcelColumnName("名称")] public string Name { get; set; }

    [ExcelColumnName("最大堆叠数")] public uint MaxCount { get; set; }
    [ExcelColumnName("##ID")] public CurrencyType Id { get; set; }
}

[Config("Item")]
public class CurrencyConfigCategory : ACategory<CurrencyType, CurrencyConfig>
{
}