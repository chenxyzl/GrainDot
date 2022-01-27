using System.Collections.Generic;
using Base.ConfigParse;
using ExcelMapper;
using Message;

namespace Base.Config;

[SheetName("MapListTest")]
public class MapListValue : IExcelConfig<uint>
{
    [ExcelIgnore] [ExcelColumnItem("ListTest")]
    public List<Item> a = new();


    [ExcelColumnName("key", field: "val")]
    [ExcelColumnMulti(5)]
    public Dictionary<uint, uint> MapTest { get; set; }

    [ExcelColumnName("list")]
    [ExcelColumnMulti(5)]
    public List<int> ListTest { get; set; }

    [ExcelColumnName("id")] public uint Id { get; set; }
}

[Config("Example")]
internal class ExampleConfigCategory : ACategory<uint, MapListValue>
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
        foreach (var v1 in v.Value.MapTest)
            v.Value.a.Add(new Item {Tid = v1.Key, Count = v1.Value});
    }
}