using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Zune.Net.Identifiers;

public class PropertyMapperRegistry
{
    private readonly List<IPropertyMapper> _mappers = [];
    private readonly IncidenceMatrix _incidenceMatrix = new();

    public IReadOnlyList<IPropertyMapper> Mappers => _mappers;

    public static PropertyMapperRegistry CreateDefault()
    {
        return new PropertyMapperRegistry()
            .RegisterMapper(new MusicBrainzIdMapper())
            .RegisterMapper(new MusicBrainzPropertyMapper())
            .RegisterMapper(new DiscogsPropertyMapper())
            .RegisterMapper(new WikidataIdMapper());
    }

    public PropertyMapperRegistry RegisterMapper(IPropertyMapper mapper)
    {
        _mappers.Add(mapper);

        foreach (var mapping in mapper.AvailableMappings)
        {
            foreach (var prop in mapping.Inputs)
                AddOrCreateIncidenceRow(prop).InputTo.Add(mapper);
            
            foreach (var prop in mapping.Outputs)
                AddOrCreateIncidenceRow(prop).OutputFrom.Add(mapper);
        }
        
        return this;
    }

    public IEnumerable<PropertyMapperHyperedge> ForInputs(IReadOnlyPropertySet inputs)
    {
        return _mappers.SelectMany(mr => 
            mr.AvailableMappings
                .Where(ma => ma.Inputs.IsSubsetOf(inputs))
                .Select(ma => new PropertyMapperHyperedge(mr, ma)));
    }
    
    public IEnumerable<IPropertyMapper> ForInput(EntityProperty input) => _incidenceMatrix[input].InputTo;

    public IEnumerable<IPropertyMapper> ForOutput(EntityProperty output) => _incidenceMatrix[output].OutputFrom;

    private IncidenceRow AddOrCreateIncidenceRow(EntityProperty property)
    {
        if (_incidenceMatrix.TryGetValue(property, out var row))
            return row;

        return _incidenceMatrix[property] = new IncidenceRow();
    }
}

[DebuggerDisplay("{Mapper.GetType().Name}: {Edge}")]
public record PropertyMapperHyperedge(IPropertyMapper Mapper, PropertyMapping Edge);