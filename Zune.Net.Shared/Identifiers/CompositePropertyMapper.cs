using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.Extensions;

namespace Zune.Net.Identifiers;

public class CompositePropertyMapper(PropertyMapperRegistry mapperRegistry)
{
    public DebugCompositePropertyMapperResult DebugInfo { get; private set; }
    
    public async Task<object> MapAsync(EntityProperty sourceProperty, object source, EntityProperty targetProperty)
    {
        var outputs = await MapAsync(sourceProperty, source, [targetProperty]);
        return outputs[targetProperty];
    }
    
    public async Task<IPropertyBag> MapAsync(EntityProperty sourceProperty,
        object source, IReadOnlyPropertySet targetProperties)
    {
        DebugInfo = new();

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
            DebugInfo.NumEdgesEvaluated++;
            
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

            // Evaluated mappings, skipping ones we've already done (doesn't matter if it succeeded)
            foreach (var hyperedge in path.Where(hyperedge => !usedHyperedges.Contains(hyperedge)))
            {
                var (mapper, mapping) = hyperedge;
                
                // Abort paths that have more mappings than necessary
                // (e.g., an edge is traversed that doesn't produce any useful properties)
                if (!mapping.Outputs.Any(desiredOutputs.Contains))
                    break;

                // Attempt to get the values we'll input to the mapper
                if (!variables.TryGetForSet(mapping.Inputs, out var edgeInputValues))
                    break;
                
                // Perform the mapping
                var outputsToRequest = mapping.Outputs.Intersect(desiredOutputs).ToPropertySet();

                IPropertyBag results;
                try
                {
                    results = await mapper.ExecuteAsync(edgeInputValues, outputsToRequest);
                }
                catch
                {
                    break;
                }
                finally
                {
                    usedHyperedges.Add(hyperedge);
                }
                
                variables.TryAddFrom(results);
                desiredOutputs.ExceptWith(mapping.Outputs);
                
                DebugInfo.TotalCost += mapping.Cost;
            }
            
            // Check if we're done
            if (variables.TryGetForSet(targetProperties, out _))
                return variables;
        }
        
        return variables;
    }
}

public class DebugCompositePropertyMapperResult
{
    public int NumEdgesEvaluated { get; set; } = 0;
    public int TotalCost { get; set; } = 0;
}