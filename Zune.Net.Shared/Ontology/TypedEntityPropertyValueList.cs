using System.Collections.Generic;

namespace Zune.Net.Ontology;

/// <summary>
/// Stores a list of values, annotated with the property they represent.
/// </summary>
public class TypedEntityPropertyValueList<T> : List<T>
{
    public required ITypedEntityProperty<T> Property { get; init; }

    public TypedEntityPropertyValueList()
    {
    }

    public TypedEntityPropertyValueList(IEnumerable<T> items) : base(items)
    {
    }
}

public static class TypedEntityPropertyValueListExtensions
{
    public static TypedEntityPropertyValueList<T> ToPropertyValueList<T>(this IEnumerable<T> items, ITypedEntityProperty<T> prop)
    {
        return new TypedEntityPropertyValueList<T>(items)
        {
            Property = prop
        };
    }
}