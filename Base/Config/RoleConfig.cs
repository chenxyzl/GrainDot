using Base.ConfigParse;
using ExcelMapper;

namespace Base.Config;

[SheetName("职业类型")]
public class RoleConfig : IExcelConfig<int>
{
    [ExcelColumnName("名字")] public string CareerName { get; set; }
    [ExcelColumnName("##ID")] public int Id { get; set; }
}

[Config("Role")]
public class RoleConfigCategory : ACategory<int, RoleConfig>
{
}