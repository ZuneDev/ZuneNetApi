using System;

namespace Zune.Net.Identifiers;

public static class IPropertySetBuilder
{
    public static IPropertySet Create(ReadOnlySpan<EntityProperty> props) => new PropertySet(props.ToArray());
}

public static class IReadOnlyPropertySetBuilder
{
    public static IReadOnlyPropertySet Create(ReadOnlySpan<EntityProperty> props) => new PropertySet(props.ToArray());
}