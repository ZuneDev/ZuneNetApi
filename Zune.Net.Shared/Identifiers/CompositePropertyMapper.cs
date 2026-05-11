using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        
        var openSet = new PriorityQueue<EntityProperty, int>();
        openSet.Enqueue(sourceProperty, 0);
        
        // TODO: Is this necessary?
        var closedSet = new HashSet<PropertyMapperNode>();
        
        var distances = new Dictionary<EntityProperty, int>
        {
            [sourceProperty] = 0
        };
        
        foreach (var targetProperty in targetProperties)
            distances[targetProperty] = int.MaxValue;
        
        var bestMapping = new Dictionary<EntityProperty, PropertyMapperNode>();

        // Process the queue until all reachable vertices are finalized
        while (openSet.TryDequeue(out var u, out var d))
        {
            var uDistance = distances.GetValueOrDefault(u, int.MaxValue);
            
            // If this distance is not the latest shortest one, skip it
            if (distances.TryGetValue(u, out var previousDistance) && d > previousDistance)
                continue;

            // Explore all adjacent vertices
            var connectedNodes = mapperRegistry
                .ForInputs([u])
                .Where(m => !closedSet.Contains(m));
            foreach (var currentNode in connectedNodes)
            {
                closedSet.Add(currentNode);
                
                var (_, currentMapping) = currentNode;
                var (w, _, vs) = currentMapping;

                foreach (var v in vs)
                {
                    NumEdgesEvaluated++;
                    var vDistance = distances.GetValueOrDefault(v, int.MaxValue);

                    // If we found a shorter path to v through u, update it
                    if (uDistance + w < vDistance)
                    {
                        vDistance = uDistance + w;
                        distances[v] = vDistance;
                        
                        openSet.Enqueue(v, vDistance);
                        bestMapping[v] = currentNode with
                        {
                            Edge = currentMapping with
                            {
                                Inputs = [u],
                                Outputs = [v]
                            }
                        };
                    }
                }
            }
        }

        Stack<PropertyMapperNode> procedure = new();
            
        PropertyBag variables = new()
        {
            [sourceProperty] = source
        };

        foreach (var targetProperty in targetProperties)
        {
            if (!bestMapping.TryGetValue(targetProperty, out var bestNode))
                throw new InvalidOperationException($"No path found from {sourceProperty} to {targetProperty}");

            do
            {
                procedure.Push(bestNode);

                var input = bestNode.Edge.Inputs.Single();
                if (input == sourceProperty)
                    break;
                
                bestNode = bestMapping[input];
            } while (bestMapping.TryGetValue(bestNode.Edge.Outputs.Single(), out bestNode));

            while (procedure.TryPop(out var nextStep))
            {
                if (variables.AsReadOnlyPropertySet().IsSupersetOf(nextStep.Edge.Outputs))
                    continue;
                
                var stepInputs = variables.GetForSet(nextStep.Edge.Inputs);
                
                var stepOutputs = await nextStep.Mapper.ExecuteAsync(stepInputs, nextStep.Edge.Outputs);
                
                TotalCost += nextStep.Edge.Cost;
                variables.TryAddFrom(stepOutputs);
            }

            procedure.Clear();
        }
        
        return variables;
    }
}