using System;

namespace Zune.Net.Ontology.BaseProperties;

public interface IEntityReferenceProperty : IEntityProperty
{
    IEntityIdProperty IdProperty { get; }
}

public record EntityReferenceProperty(EntityType EntityType, EntityFact Fact, IEntityIdProperty IdProperty)
    : IEntityReferenceProperty;

/// <summary>
/// Represents a property that is a reference to another entity using its identifier.
/// </summary>
/// <param name="EntityType">The type of the entity this property applies to.</param>
/// <param name="Fact">The fact on the entity this property represents.</param>
/// <param name="TypedIdProperty">The property on the referenced entity that contains the identifier.</param>
/// <typeparam name="TId">The type of the identifier for the referenced entity.</typeparam>
/// <typeparam name="TProviderEntities">
/// An enum determining what kind of provider entity it applies to.
/// Implicitly specifies who assigned this ID, e.g. MusicBrainz or Discogs.
/// </typeparam>
public record TypedEntityReferenceProperty<TId, TProviderEntities>(EntityType EntityType, EntityFact Fact,
    ITypedEntityIdProperty<TId, TProviderEntities> TypedIdProperty)
    : EntityProperty(EntityType, Fact), IEntityReferenceProperty
    where TProviderEntities : Enum
{
    public override int GetHashCode() => base.GetHashCode();
    
    protected override Type EqualityContract => base.EqualityContract;
    
    public override string ToString() => $"{base.ToString()}<{IdProperty}>";
    
    public IEntityIdProperty IdProperty => TypedIdProperty;
}