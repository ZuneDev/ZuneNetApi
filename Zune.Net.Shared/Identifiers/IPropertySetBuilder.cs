using System;

namespace Zune.Net.Identifiers;

public static class IPropertySetBuilder
{
    public static IPropertySet Create(ReadOnlySpan<IEntityProperty> props) => new PropertySet(props.ToArray());
}

public static class IReadOnlyPropertySetBuilder
{
    public static IReadOnlyPropertySet Create(ReadOnlySpan<IEntityProperty> props) => new PropertySet(props.ToArray());
}