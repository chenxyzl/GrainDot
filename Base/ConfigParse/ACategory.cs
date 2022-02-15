using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using ExcelMapper;
using Message;

namespace Base.ConfigParse;

public abstract class ACategory : ISupportInitialize
{
    public abstract Type ConfigType { get; }

    public virtual void BeginInit()
    {
    }

    public virtual void EndInit()
    {
    }
}

/// <summary>
///     管理该所有的配置
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ACategory<K, T> : ACategory where T : IExcelConfig<K> where K : notnull
{
    protected Dictionary<K, T> dict = new();

    public override Type ConfigType => typeof(T);

    public override void BeginInit()
    {
        //typeof(T).GetTypeInfo().IsAssignableFrom(typeof(IExcelConfig).GetTypeInfo())
        var fileName = A.NotNull(GetType().GetCustomAttribute<ConfigAttribute>()?.FileName, Code.Error,
            "get ConfigAttribute not found");
        FromExcel(fileName);
    }

    private void FromExcel(string fileName)
    {
        var temp = new Dictionary<K, T>();
        var path = $"../Config/{fileName}.xlsx";
        try
        {
            using (var stream = File.OpenRead(path))
            {
                using var importer = new ExcelImporter(stream);
                var sheetNames = A.NotNull(typeof(T).GetCustomAttribute<SheetNameAttribute>()?.SheetName,
                    Code.Error, "get SheetNameAttribute not found");
                foreach (var sheetName in sheetNames)
                {
                    var sheet = importer.ReadSheet(sheetName);
                    var president = sheet.ReadRows<T>().ToArray();
                    foreach (var v in president) temp[v.Id] = v;
                }
            }

            dict = temp;
        }
        catch (Exception e)
        {
            throw new Exception($"parser excel fail: {path}", e);
        }
    }

    public override void EndInit()
    {
    }

    public T Get(K id)
    {
        if (!dict.TryGetValue(id, out var t))
            t = A.NotNull(t, Code.ConfigNotFound, $"not found config: {typeof(T)} id: {id}");

        return t;
    }

    public Dictionary<K, T> GetAll()
    {
        return dict;
    }

    public T GetOne()
    {
        return dict.Values.First();
    }
}

/// <summary>
///     管理该所有的配置
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ACategory<K, T, V> : ACategory
    where T : ExcelClassMap<V> where V : IExcelConfig<K> where K : notnull
{
    protected Dictionary<K, V> dict = new();

    public override Type ConfigType => typeof(V);

    public override void BeginInit()
    {
        //typeof(T).GetTypeInfo().IsAssignableFrom(typeof(IExcelConfig).Ge‌​tTypeInfo())
        var fileName = A.NotNull(GetType().GetCustomAttribute<ConfigAttribute>()?.FileName, Code.Error,
            "get ConfigAttribute not found");
        FromExcel(fileName);
    }

    private void FromExcel(string fileName)
    {
        var temp = new Dictionary<K, V>();
        var path = $"../Config/{fileName}.xlsx";
        try
        {
            using (var importer = new ExcelImporter(File.OpenRead(path)))
            {
                importer.Configuration.RegisterClassMap(Activator.CreateInstance<T>());
                var sheetNames = A.NotNull(typeof(V).GetCustomAttribute<SheetNameAttribute>()?.SheetName,
                    Code.Error, "get SheetNameAttribute not found");
                foreach (var sheetName in sheetNames)
                {
                    var sheet = importer.ReadSheet(sheetName);
                    var president = sheet.ReadRows<V>().ToArray();
                    foreach (var v in president) temp[v.Id] = v;
                }
            }

            dict = temp;
        }
        catch (Exception e)
        {
            throw new Exception($"parser excel fail: {path}", e);
        }
    }

    public override void EndInit()
    {
    }

    public V Get(K id)
    {
        if (!dict.TryGetValue(id, out var t))
            t = A.NotNull(t, Code.ConfigNotFound, $"not found config: {typeof(V)} id: {id}");

        return t;
    }

    public Dictionary<K, V> GetAll()
    {
        return dict;
    }

    public V GetOne()
    {
        return dict.Values.First();
    }
}