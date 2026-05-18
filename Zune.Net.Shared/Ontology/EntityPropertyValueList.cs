using System.Collections.Generic;
using Zune.Net.Ontology.BaseProperties;

namespace Zune.Net.Ontology;

/// <summary>
/// Stores a list of values, annotated with the property they represent.
/// </summary>
public class EntityPropertyValueList : List<object>
{
    public required IEntityProperty Property { get; init; }

    public EntityPropertyValueList()
    {
    }

    public EntityPropertyValueList(IEnumerable<object> items) : base(items)
    {
    }
}

public static class EntityPropertyValueListExtensions
{
    public static EntityPropertyValueList ToPropertyValueList(this IEnumerable<object> items, IEntityProperty prop)
    {
        return new EntityPropertyValueList(items)
        {
            Property = prop
        };
    }
}