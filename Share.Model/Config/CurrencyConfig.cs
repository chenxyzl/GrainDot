
using ExcelMapper;
using Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Message;

namespace Share.Model
{

    [SheetName("货币")]
    public class CurrencyConfig : IExcelConfig<CurrencyType>
    {
        [ExcelColumnName("##ID")]
        public CurrencyType Id { get; set; }

        [ExcelColumnName("名称")]
        public string Name { get; set; }

        [ExcelColumnName("最大堆叠数")]
        public uint MaxCount { get; set; }
    }

    [Config("Item")]
    public partial class CurrencyConfigCategory : ACategory<CurrencyType, CurrencyConfig> { }
}
