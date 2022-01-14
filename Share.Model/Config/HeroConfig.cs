
using ExcelMapper;
using Base;
using Message;

namespace Share.Model
{

    [SheetName("英雄")]
    public class HeroConfig : IExcelConfig<int>
    {
        [ExcelColumnName("##ID")]
        public int Id { get; set; }

        [ExcelColumnName("名称")]
        public string Name { get; set; }

        [ExcelColumnName("稀有度")]
        public Rarity Rarity { get; set; }
    }

    [Config("Hero")]
    public partial class HeroConfigCategory : ACategory<int, HeroConfig>
    {
        public static HeroConfigCategory Instance;
        public HeroConfigCategory()
        {
            Instance = this;
        }
    }
}
