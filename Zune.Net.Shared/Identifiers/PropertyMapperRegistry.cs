using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Zune.Net.Identifiers;

public class PropertyMapperRegistry
{
    private readonly List<IPropertyMapper> _mappers = [];

    public IReadOnlyList<IPropertyMapper> Mappers => _mappers;

    public static PropertyMapperRegistry CreateDefault()
    {
        return new PropertyMapperRegistry()
            .RegisterMapper(new MusicBrainzPropertyMapper())
            .RegisterMapper(new DiscogsPropertyMapper())
            .RegisterMapper(new WikidataIdMapper());
    }

    public PropertyMapperRegistry RegisterMapper(IPropertyMapper mapper)
    {
        _mappers.Add(mapper);
        return this;
    }

    public IEnumerable<PropertyMapperHyperedge> ForInputs(IReadOnlyPropertySet inputs)
    {
        return _mappers.SelectMany(mr => 
            mr.AvailableMappings
                .Where(ma => ma.Inputs.IsSubsetOf(inputs))
                .Select(ma => new PropertyMapperHyperedge(mr, ma)));
    }
}

[DebuggerDisplay("{Mapper.GetType().Name}: {Edge}")]
public record PropertyMapperHyperedge(IPropertyMapper Mapper, PropertyMapping Edge);