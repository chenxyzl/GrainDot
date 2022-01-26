using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using Message;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace Base.State;

public abstract class BaseState : ISupportInitialize
{
    //是否需要保存
    [IgnoreDataMember] [BsonIgnore] public abstract bool NeedSave { get; protected set; }

    //脏标记
    [IgnoreDataMember] [BsonIgnore] public bool Dirty { get; private set; } = false;

    public void CleanDirty()
    {
        Dirty = false;
    }

    public void MarkDirty()
    {
        Dirty = true;
    }

    //挂载的组建
    [IgnoreDataMember] [BsonIgnore] private IActorComponent _component;

    public void BindComponent(IActorComponent actorComponent)
    {
        _component = actorComponent;
    }

    public T GetComponent<T>() where T : IActorComponent
    {
        return _component as T;
    }

    public virtual void BeginInit()
    {
    }

    public virtual void EndInit()
    {
    }
}