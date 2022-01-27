using System.ComponentModel;
using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Base.State;

/// <summary>
///     state独立的原因是为了分离数据和逻辑
///     state不能热更，所以这里不要些逻辑方法，只允许get set
///     后期考虑增加load时候的检测
/// </summary>
public abstract class BaseState : ISupportInitialize
{
    //挂载的组建
    [IgnoreDataMember] [BsonIgnore] private IActorComponent _component;

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

    public void BindComponent(IActorComponent actorComponent)
    {
        _component = actorComponent;
    }

    public T GetComponent<T>() where T : IActorComponent
    {
        return _component as T;
    }
}