using System;
using Zune.Net.Models;

namespace Zune.Net.Ontology.BaseProperties;

public interface IEntityIdProperty : IParsableEntityProperty;

public record EntityIdProperty(EntityType EntityType, EntityFact Fact)
    : EntityProperty(EntityType, Fact), IEntityIdProperty
{
    public object ParseObject(string value) => value;
}

public interface ITypedEntityIdProperty<out TIdValue, out TProviderEntities>
    : IParsableTypedEntityProperty<TIdValue>, IEntityIdProperty
    where TProviderEntities : Enum
{
    /// <summary>
    /// The type of the entity this property applies to, specific to the provider of this identifier.
    /// </summary>
    TProviderEntities ProviderEntityType { get; }
}

public abstract class TypedEntityIdProperty<TIdValue, TProviderEntities>
    (TProviderEntities providerEntityType, EntityType entityType)
    : ITypedEntityIdProperty<TIdValue, TProviderEntities>
    where TProviderEntities : Enum
{
    public TProviderEntities ProviderEntityType { get; } = providerEntityType;
    public EntityType EntityType { get; } = entityType;
    public EntityFact Fact => EntityFact.Id;

    public abstract TIdValue Parse(string value);
    public virtual object ParseObject(string value) => Parse(value);

    // TODO: Is this actually useful?
    public virtual IIdentifier<TIdValue, TProviderEntities> CreateIdentifier(TIdValue id) =>
        new Identifier<TIdValue, TProviderEntities>(id, ProviderEntityType);

    public override bool Equals(object otherProp)
    {
        if (otherProp is null)
            return false;
        
        if (ReferenceEquals(this, otherProp))
            return true;
        
        if (otherProp is not TypedEntityIdProperty<TIdValue, TProviderEntities> otherTypedIdProp)
            return false;
        
        return ProviderEntityType.Equals(otherTypedIdProp.ProviderEntityType)
            && EntityType == otherTypedIdProp.EntityType
            && Fact == otherTypedIdProp.Fact;
    }

    public override int GetHashCode() => (ProviderEntityType, EntityType, Fact).GetHashCode();
    
    public override string ToString() => $"{typeof(TProviderEntities).Name}:{ProviderEntityType}.{Fact}";
}