using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.Extensions;

using Hyperpath = System.Collections.Immutable.ImmutableList<Zune.Net.Ontology.PropertyMapperHyperedge>;
using RankedHyperpath = System.Linq.IOrderedEnumerable<System.Collections.Immutable.ImmutableList<Zune.Net.Ontology.PropertyMapperHyperedge>>;

namespace Zune.Net.Ontology;

public class CompositePropertyMapper(PropertyMapperRegistry mapperRegistry)
{
    private readonly Dictionary<(IReadOnlyPropertySet, IReadOnlyPropertySet), RankedHyperpath> _pathsCache = new();
    
    public DebugCompositePropertyMapperResult DebugInfo { get; private set; }
    
    public async Task<object> MapAsync(IEntityProperty sourceProperty, object source, IEntityProperty targetProperty)
    {
        var outputs = await MapAsync(sourceProperty, source, [targetProperty]);
        return outputs[targetProperty];
    }
    
    public async Task<TTarget> MapAsync<TSource, TTarget>(TypedEntityProperty<TSource> sourceProperty, TSource source,
        TypedEntityProperty<TTarget> targetProperty)
    {
        var outputs = await MapAsync(sourceProperty, source, [targetProperty]);
        return outputs.Get(targetProperty);
    }

    public async Task<IPropertyBag> MapAsync<TSource>(TypedEntityProperty<TSource> sourceProperty,
        TSource source, IReadOnlyPropertySet targetProperties)
    {
        return await MapAsync((IEntityProperty)sourceProperty, source, targetProperties);
    }
    
    public async Task<IPropertyBag> MapAsync(IEntityProperty sourceProperty,
        object source, IReadOnlyPropertySet targetProperties)
    {
        DebugInfo = new();
        
        PropertyBag variables = new()
        {
            [sourceProperty] = source
        };

        var rankedPaths = GetOrComputeMap(
            variables.AsReadOnlyPropertySet(),
            targetProperties);

        HashSet<PropertyMapperHyperedge> usedHyperedges = [];

        foreach (var path in rankedPaths)
        {
            var remainingOutputs = targetProperties
                .Union(path.SelectMany(n => n.Edge.Inputs))
                .ToHashSet();
            remainingOutputs.Remove(sourceProperty);

            // Evaluated mappings, skipping ones we've already done (doesn't matter if it succeeded)
            foreach (var hyperedge in path.Where(hyperedge => !usedHyperedges.Contains(hyperedge)))
            {
                var (mapper, mapping) = hyperedge;
                
                // Abort paths that have more mappings than necessary
                // (e.g., an edge is traversed that doesn't produce any useful properties)
                if (!mapping.Outputs.Any(remainingOutputs.Contains))
                    break;

                // Attempt to get the values we'll input to the mapper
                if (!variables.TryGetForSet(mapping.Inputs, out var edgeInputValues))
                    break;
                
                // Only request the properties we need
                var outputsToRequest = mapping.Outputs.Intersect(remainingOutputs).ToPropertySet();

                // Perform the mapping
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
                
                // Save results and mark outputs as done
                variables.TryAddFrom(results);
                remainingOutputs.ExceptWith(mapping.Outputs);
                
                DebugInfo.TotalCost += mapping.Cost;
                ++DebugInfo.NumEdgesExecuted;
            }
            
            // Check if we're done
            if (variables.TryGetForSet(targetProperties, out _))
                return variables;
        }
        
        return variables;
    }
    
    public RankedHyperpath GetOrComputeMap(
        IReadOnlyPropertySet sourceProperties, IReadOnlyPropertySet targetProperties)
    {
        var key = (sourceProperties, targetProperties);
        if (_pathsCache.TryGetValue(key, out var paths))
            return paths;
        
        return _pathsCache[key] = ComputeMap(sourceProperties, targetProperties);
    }

    private RankedHyperpath ComputeMap(
        IReadOnlyPropertySet sourceProperties, IReadOnlyPropertySet targetProperties)
    {
        var initialPaths = mapperRegistry
            .ForInputs(sourceProperties)
            .Select(n => n.IntoList().ToImmutableList());
        
        var incompletePaths = new Queue<Hyperpath>(initialPaths);
        var pathsOfInterest = new HashSet<Hyperpath>(new EnumerableEqualityComparer<PropertyMapperHyperedge>());
        
        // Explore paths, noting ones that produce any of the target properties
        while (incompletePaths.TryDequeue(out var incompletePath))
        {
            ++DebugInfo.NumEdgesTested;
            
            var currentOutputs = incompletePath
                .SelectMany(n => n.Edge.Outputs)
                .ToPropertySet();
                
            // Strict metric, we only care about paths that produce all target properties
            if (targetProperties.IsSubsetOf(currentOutputs))
                pathsOfInterest.Add(incompletePath);
            
            currentOutputs.UnionWith(sourceProperties);
            
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

        return pathsOfInterest.OrderBy(p => p.Sum(h => h.Edge.Cost));
    }
}

public class DebugCompositePropertyMapperResult
{
    public int NumEdgesTested { get; set; } = 0;
    public int NumEdgesExecuted { get; set; } = 0;
    public int TotalCost { get; set; } = 0;
}