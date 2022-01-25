namespace Base.ConfigParse;

[AttributeUsage(AttributeTargets.Class)]
public class ConfigAttribute : BaseAttribute
{
    public ConfigAttribute(string fileName)
    {
        FileName = fileName;
    }

    public string FileName { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class SheetNameAttribute : BaseAttribute
{
    public SheetNameAttribute(string[] sheetNames)
    {
        SheetName = sheetNames;
    }

    public SheetNameAttribute(string sheetName)
    {
        SheetName = new[] {sheetName};
    }

    public string[] SheetName { get; }
}