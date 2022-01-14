namespace Base
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConfigAttribute : BaseAttribute
    {
        public string FileName { get; }

        public ConfigAttribute(string fileName)
        {
            FileName = fileName;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SheetNameAttribute : BaseAttribute
    {
        public string[] SheetName { get; }

        public SheetNameAttribute(string[] sheetNames)
        {
            SheetName = sheetNames;
        }
        public SheetNameAttribute(string sheetName)
        {
            SheetName = new string[] { sheetName };
        }
    }
}