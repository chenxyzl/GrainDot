using System.ComponentModel;
using System.Runtime.Serialization;
using Common;
using MongoDB.Bson.Serialization.Attributes;

namespace Base.State;

/// <summary>
///     state独立的原因是为了分离数据和逻辑
///     state不能热更，所以这里不要些逻辑方法，只允许get set
///     后期考虑增加load时候的检测
/// </summary>
public abstract class BaseState : ISupportInitialize
{
    [BsonElement] [BsonId] public ulong Id;

    //todo 之后要实现更新DBVersion来升级db数据
    public DBVersion Version = DBVersion.Null;

    //是否需要保存
    [IgnoreDataMember] [BsonIgnore] public abstract bool NeedSave { get; protected set; }

    //脏标记
    [IgnoreDataMember] [BsonIgnore] public bool Dirty { get; private set; }

    public virtual void BeginInit()
    {
    }

    public virtual void EndInit()
    {
    }

    public void CleanDirty()
    {
        Dirty = false;
    }

    public void MarkDirty()
    {
        Dirty = true;
    }
}