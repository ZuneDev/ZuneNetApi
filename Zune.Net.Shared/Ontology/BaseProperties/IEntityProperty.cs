namespace Zune.Net.Ontology.BaseProperties;

public interface IEntityProperty
{
    EntityType EntityType { get; }
    EntityFact Fact { get; }
}

public record EntityProperty(EntityType EntityType, EntityFact Fact) : IEntityProperty
{
    public override string ToString() => $"{EntityType}.{Fact}";
}

public interface ITypedEntityProperty<out T> : IEntityProperty;

public record TypedEntityProperty<T>(EntityType EntityType, EntityFact Fact) : ITypedEntityProperty<T>;