using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Zune.Net.Ontology.Mappers;

namespace Zune.Net.Ontology;

public class PropertyMapperRegistry
{
    private readonly List<IPropertyMapper> _mappers = [];

    public IReadOnlyList<IPropertyMapper> Mappers => _mappers;

    public static PropertyMapperRegistry CreateDefault()
    {
        return new PropertyMapperRegistry()
            .RegisterMappers([
                new MusicBrainzPropertyMapper(),
                new DiscogsPropertyMapper(),
                new WikidataIdMapper(),
            ]);
    }

    public PropertyMapperRegistry RegisterMapper(IPropertyMapper mapper)
    {
        _mappers.Add(mapper);
        return this;
    }

    public PropertyMapperRegistry RegisterMappers(IEnumerable<IPropertyMapper> mappers)
    {
        _mappers.AddRange(mappers);
        return this;
    }

    public IEnumerable<PropertyMapperHyperedge> ForInputs(IReadOnlyPropertySet inputs) =>
        GetMappings(ma => ma.Inputs.IsSubsetOf(inputs));

    public IEnumerable<PropertyMapperHyperedge> GetMappings(Func<PropertyMapping, bool> predicate = null)
    {
        predicate ??= _ => true;
        
        return _mappers.SelectMany(mr =>
            mr.AvailableMappings
                .Where(predicate)
                .Select(ma => new PropertyMapperHyperedge(mr, ma)));
    }
}

[DebuggerDisplay("{Mapper.GetType().Name}: {Edge}")]
public record PropertyMapperHyperedge(IPropertyMapper Mapper, PropertyMapping Edge);