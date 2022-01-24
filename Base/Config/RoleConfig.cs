
using ExcelMapper;
using Base.ConfigParse;

namespace Base.Config
{

    [SheetName("职业类型")]
    public class RoleConfig : IExcelConfig<int>
    {
        [ExcelColumnName("##ID")]
        public int Id { get; set; }

        [ExcelColumnName("名字")]
        public string CareerName { get; set; }
    }

    [Config("Role")]
    public partial class RoleConfigCategory : ACategory<int, RoleConfig> { }
}
