using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Base;

public abstract class IActorComponent : IComponent
{
    public BaseActor Node;

    public IActorComponent(BaseActor a)
    {
        Node = a;
    }
}