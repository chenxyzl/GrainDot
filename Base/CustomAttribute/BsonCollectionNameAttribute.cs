namespace Base;

[AttributeUsage(AttributeTargets.Class)]
public class BsonCollectionNameAttribute : BaseAttribute
{
    public BsonCollectionNameAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}