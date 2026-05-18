using System;

namespace Zune.Net.Ontology.BaseProperties;

public record TypedEntityReferenceListProperty<TIdValue, TProviderEntities> : TypedEntityListProperty<TIdValue>
    where TProviderEntities : Enum
{
    public TypedEntityReferenceListProperty(EntityType entityType, EntityFact fact,
        ITypedEntityIdProperty<TIdValue, TProviderEntities> idProperty)
        : base(entityType, fact, idProperty)
    {
    }

    public override int GetHashCode() => base.GetHashCode();

    protected override Type EqualityContract => base.EqualityContract;

    public override string ToString() => base.ToString();
}