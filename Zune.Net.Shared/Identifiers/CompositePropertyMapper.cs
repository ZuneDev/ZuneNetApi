using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.ComponentModel;
using OwlCore.Extensions;

namespace Zune.Net.Identifiers;

public class CompositePropertyMapper(PropertyMapperRegistry mapperRegistry)
{
    public async Task<TTarget> MapOneToOneAsync<TSource, TTarget>(EntityType entity, EntityPropertyType sourceProperty,
        TSource source, EntityPropertyType targetProperty)
    {
        Dictionary<EntityProperty, IncidenceRow> incidenceMatrix = new();
        
        PropertyBag variables = new()
        {
            [new EntityProperty(entity, sourceProperty)] = source
        };

        var openSet = new PriorityQueue<ISet<EntityProperty>, int>();

        var cameFrom = new Dictionary<ISet<EntityProperty>, ISet<EntityProperty>>();

        var gScore = new Dictionary<ISet<EntityProperty>, int>();
        foreach (var sourceProp in variables.Keys)
            gScore[sourceProp.IntoList().ToHashSet()] = 0;

        do
        {
            throw new NotImplementedException();
        } while (true);
    }
}

internal record IncidenceRow(HashSet<IPropertyMapper> HeadsOf, HashSet<IPropertyMapper> TailsOf)
{
    public IncidenceRow() : this(new(), new())
    {
    }
}