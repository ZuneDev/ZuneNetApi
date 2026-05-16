using System.Linq;
using System.Text;

namespace Zune.Net.Ontology;

public class OntologyVisualizer(PropertyMapperRegistry registry)
{
    public string SerializeToGraphviz(bool useDigraph = true)
    {
        var mappings = registry.GetMappings().ToList();
        var properties = mappings
            .SelectMany(m => m.Edge.Inputs)
            .Union(mappings.SelectMany(m => m.Edge.Outputs))
            .Distinct();

        StringBuilder sb = new();
        string edgeSymbol;

        if (useDigraph)
        {
            edgeSymbol = "->";
            
            sb.AppendLine("digraph G {");
        }
        else
        {
            edgeSymbol = "--";
            
            sb.AppendLine("graph G {");
            sb.AppendLine("    splines=true;");
        }

        sb.AppendLine("    node [shape=box];");
        sb.AppendLine();

        foreach (var property in properties)
            sb.AppendLine($"    n{property.GetHashCode():X} [label=\"{property}\"];");

        sb.AppendLine();
        sb.AppendLine("    node [shape=plain];");
        sb.AppendLine();
        
        foreach (var hyperedge in mappings)
        {
            var edgeId = $"e{hyperedge.GetHashCode():X}";
            var edgeName = hyperedge.Mapper.GetType().Name;
            
            sb.AppendLine($"    {edgeId} [label=\"{edgeName} ({hyperedge.Edge.Cost})\"];");

            foreach (var input in hyperedge.Edge.Inputs)
                sb.AppendLine($"    n{input.GetHashCode():X} {edgeSymbol} {edgeId};");

            foreach (var output in hyperedge.Edge.Outputs)
                sb.AppendLine($"    {edgeId} {edgeSymbol} n{output.GetHashCode():X};");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }
}