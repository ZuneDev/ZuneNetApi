using System;
using Zune.Net.Ontology.BaseProperties;

namespace Zune.Net.Ontology;

public static class IReadOnlyPropertySetBuilder
{
    public static IReadOnlyPropertySet Create(ReadOnlySpan<IEntityProperty> props) => new PropertySet(props.ToArray());
}