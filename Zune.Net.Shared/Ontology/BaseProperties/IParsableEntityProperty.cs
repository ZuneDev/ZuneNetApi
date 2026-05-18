using System;
using System.ComponentModel;

namespace Zune.Net.Ontology.BaseProperties;

public interface IParsableEntityProperty : IEntityProperty
{
    object ParseObject(string value);
}

public interface IParsableTypedEntityProperty<out T> : ITypedEntityProperty<T>, IParsableEntityProperty
{
    T Parse(string value);
}

public record ParsableTypedEntityProperty<T>(EntityType EntityType, EntityFact Fact)
    : EntityProperty(EntityType, Fact), IParsableTypedEntityProperty<T>
{
    public virtual T Parse(string value)
    {
        var targetType = typeof(T);
        if (targetType == typeof(string))
            return (T)(object)value;
        
        var converter = TypeDescriptor.GetConverter(targetType);
        var result = converter.ConvertFromInvariantString(value);
        return (T)result;
    }
    
    public virtual object ParseObject(string value) => Parse(value);

    public override int GetHashCode() => base.GetHashCode();

    protected override Type EqualityContract => base.EqualityContract;
    
    public override string ToString() => base.ToString();
}

public record EntityStringProperty(EntityType EntityType, EntityFact Fact)
    : ParsableTypedEntityProperty<string>(EntityType, Fact)
{
    public override string Parse(string value) => value;
    
    public override object ParseObject(string value) => value;

    public override int GetHashCode() => base.GetHashCode();

    protected override Type EqualityContract => base.EqualityContract;
    
    public override string ToString() => base.ToString();
}