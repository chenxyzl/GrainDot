namespace Base
{
    /// <summary>
    /// 每个Config的基类
    /// </summary>
    public interface IConfig<K>
    {
        K Id { get; set; }
    }

    public interface IExcelConfig<K> : IConfig<K>
    {

    }
}