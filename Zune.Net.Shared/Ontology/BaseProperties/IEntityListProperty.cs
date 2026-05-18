using System;
using System.Collections.Generic;

namespace Zune.Net.Ontology.BaseProperties;

public record EntityListProperty(EntityType EntityType, EntityFact Fact, IEntityProperty ForeignProperty)
    : EntityProperty(EntityType, Fact), ITypedEntityProperty<IEnumerable<object>>
{
    public override int GetHashCode() => base.GetHashCode();

    protected override Type EqualityContract => base.EqualityContract;
    
    public override string ToString() => base.ToString();
}

public record TypedEntityListProperty<T>(EntityType EntityType, EntityFact Fact, ITypedEntityProperty<T> ForeignProperty)
    : EntityProperty(EntityType, Fact), ITypedEntityProperty<IEnumerable<T>>
{
    public override int GetHashCode() => base.GetHashCode();

    protected override Type EqualityContract => base.EqualityContract;
    
    public override string ToString() => base.ToString();
}