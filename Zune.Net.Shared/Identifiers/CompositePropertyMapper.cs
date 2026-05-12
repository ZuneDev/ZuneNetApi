using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.Extensions;

namespace Zune.Net.Identifiers;

public class CompositePropertyMapper(PropertyMapperRegistry mapperRegistry)
{
    public int NumEdgesEvaluated { get; private set; }
    public int TotalCost { get; private set; }
    
    public async Task<object> MapOneToOneAsync(EntityType entity, EntityPropertyType sourcePropertyType,
        object source, EntityPropertyType targetPropertyType)
    {
        var targetProperty = new EntityProperty(entity, targetPropertyType);
        var outputs = await MapOneToManyAsync(entity, sourcePropertyType, source,
            [targetProperty]);
        return outputs[targetProperty];
    }
    
    public async Task<IPropertyBag> MapOneToManyAsync(EntityType entity, EntityPropertyType sourcePropertyType,
        object source, IReadOnlyPropertySet targetProperties)
    {
        // TODO: Remove debug properties
        NumEdgesEvaluated = 0;
        TotalCost = 0; 
        
        var sourceProperty = new EntityProperty(entity, sourcePropertyType);

        var initialPaths = mapperRegistry
            .ForInputs([sourceProperty])
            .Select(n => n.IntoList().ToImmutableList())
            .ToList();
        
        var incompletePaths = new Queue<ImmutableList<PropertyMapperHyperedge>>(initialPaths);
        
        var pathsOfInterest = new HashSet<ImmutableList<PropertyMapperHyperedge>>(
            new EnumerableEqualityComparer<PropertyMapperHyperedge>());
        
        // Explore paths, noting ones that produce any of the target properties
        while (incompletePaths.TryDequeue(out var incompletePath))
        {
            NumEdgesEvaluated++;
            
            var currentOutputs = incompletePath
                .SelectMany(n => n.Edge.Outputs)
                .ToPropertySet();
                
            // Strict metric, we only care about paths that produce all target properties
            if (targetProperties.IsSubsetOf(currentOutputs))
                pathsOfInterest.Add(incompletePath);
            
            currentOutputs.Add(sourceProperty);
            
            // Explore all connecting edges
            var connectedHyperedges = mapperRegistry.ForInputs(currentOutputs);
            
            foreach (var currentHyperedge in connectedHyperedges)
            {
                // Ignore edges we've already traversed
                if (incompletePath.Contains(currentHyperedge))
                    continue;
                
                // Ignore edges that only provide outputs we already have
                if (currentHyperedge.Edge.Outputs.IsSubsetOf(currentOutputs))
                    continue;

                var newPath = incompletePath.Add(currentHyperedge);
                incompletePaths.Enqueue(newPath);

                // Alternative, less strict metric; we consider paths that produce some subset of the target properties
                // var newOutputs = newPath
                //     .SelectMany(n => n.Edge.Outputs)
                //     .ToPropertySet();
                
                // var newOutputOverlap = newOutputs.Intersect(targetProperties).Count();
                // var currentOutputOverlap = currentOutputs.Intersect(targetProperties).Count();
                //
                // if (newOutputOverlap > currentOutputOverlap)
                //     pathsOfInterest.Add(newPath);
            }
        }
        
        PropertyBag variables = new()
        {
            [sourceProperty] = source
        };

        HashSet<PropertyMapperHyperedge> usedHyperedges = [];

        var rankedPaths = pathsOfInterest.OrderBy(p => p.Sum(h => h.Edge.Cost));
        foreach (var path in rankedPaths)
        {
            var desiredOutputs = targetProperties
                .Union(path.SelectMany(n => n.Edge.Inputs))
                .ToHashSet();
            desiredOutputs.Remove(sourceProperty);

            foreach (var hyperedge in path)
            {
                // We've already performed this mapping. Either it failed or it didn't; either way, skip it.
                if (usedHyperedges.Contains(hyperedge))
                    continue;
                
                var (mapper, mapping) = hyperedge;
                
                // Abort paths that have more mappings than necessary
                // (e.g. an edge is traversed that doesn't produce any useful properties)
                if (!mapping.Outputs.Any(desiredOutputs.Contains))
                    break;

                // Attempt to get the values we'll input to the mapper
                if (!variables.TryGetForSet(mapping.Inputs, out var edgeInputValues))
                    break;
                
                // Perform the mapping
                var outputsToRequest = mapping.Outputs.Intersect(desiredOutputs).ToPropertySet();
                var results = await mapper.ExecuteAsync(edgeInputValues, outputsToRequest);
                variables.TryAddFrom(results);
                
                usedHyperedges.Add(hyperedge);
                
                desiredOutputs.ExceptWith(mapping.Outputs);
                
                TotalCost += mapping.Cost;
            }
            
            // Check if we're done
            if (variables.TryGetForSet(targetProperties, out _))
                return variables;
        }
        
        return variables;
    }
}