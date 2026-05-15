using System.Collections.Generic;

namespace Zune.Net.Identifiers;

/// <summary>
/// Stores a list of values, annotated with the property they represent.
/// </summary>
public class EntityPropertyValueList<T> : List<T>
{
    public required EntityProperty Property { get; init; }

    public EntityPropertyValueList()
    {
    }

    public EntityPropertyValueList(IEnumerable<T> items) : base(items)
    {
    }
}

public static class EntityPropertyValueListExtensions
{
    public static EntityPropertyValueList<T> ToPropertyValueList<T>(this IEnumerable<T> items, EntityProperty prop)
    {
        return new EntityPropertyValueList<T>(items)
        {
            Property = prop
        };
    }
}