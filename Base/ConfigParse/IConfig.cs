namespace Base.ConfigParse;

/// <summary>
///     每个Config的基类
/// </summary>
public interface IExcelConfig<K>
{
    K Id { get; set; }
}