using System.Collections.Generic;
using System.Linq;

namespace Zune.Net.Identifiers;

public class PropertyMapperRegistry
{
    private readonly List<IPropertyMapper> _mappers = [];

    public IReadOnlyList<IPropertyMapper> Mappers => _mappers;

    public static PropertyMapperRegistry CreateDefault()
    {
        return new PropertyMapperRegistry()
            .RegisterMapper(new WikidataIdMapper());
    }

    public PropertyMapperRegistry RegisterMapper(IPropertyMapper mapper)
    {
        _mappers.Add(mapper);
        return this;
    }

    public IEnumerable<PropertyMapperNode> ForInputs(IReadOnlyPropertySet inputs)
    {
        return _mappers.SelectMany(mr => 
            mr.AvailableMappings
                .Where(ma => ma.Inputs.IsSupersetOf(inputs))
                .Select(ma => new PropertyMapperNode(mr, ma)));
    }
}

public record PropertyMapperNode(IPropertyMapper Mapper, PropertyMapping Edge);