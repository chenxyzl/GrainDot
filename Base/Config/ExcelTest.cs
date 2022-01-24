
using Base.ConfigParse;
using ExcelMapper;
using Message;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Base.Config
{
    [SheetName("MapListTest")]
    public partial class MapListValue : IExcelConfig<int>
    {
        [ExcelColumnName("id")]
        public int Id { get; set; }


        [ExcelColumnName("key", field: "val")]
        [ExcelColumnMulti(5)]
        public Dictionary<int, int> MapTest { get; set; }

        [ExcelColumnName("list")]
        [ExcelColumnMulti(5)]
        public List<int> ListTest { get; set; }

        [ExcelIgnore]
        [ExcelColumnItem("ListTest")]
        public List<Item> a = new List<Item>();
    }

    [Config("Example")]
    partial class ExampleConfigCategory : ACategory<int, MapListValue>
    {
        public override void BeginInit()
        {
            base.BeginInit();
        }

        public override void EndInit()
        {
            base.EndInit();

            //有空了考虑用注解来实现
            foreach (var v in dict)
            {
                foreach (var v1 in v.Value.MapTest)
                {
                    v.Value.a.Add(new Item { Tid = v1.Key, Count = v1.Value });
                }
            }
        }
    }
}
