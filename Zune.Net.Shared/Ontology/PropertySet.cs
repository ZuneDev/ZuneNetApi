using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Zune.Net.Ontology.BaseProperties;

namespace Zune.Net.Ontology;

[CollectionBuilder(typeof(IReadOnlyPropertySetBuilder), nameof(IReadOnlyPropertySetBuilder.Create))]
public interface IReadOnlyPropertySet : IReadOnlySet<IEntityProperty>;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class PropertySet : HashSet<IEntityProperty>, IReadOnlyPropertySet
{
    private string DebuggerDisplay => ToString();
    
    public PropertySet()
    {
    }
    
    public PropertySet(IEnumerable<IEntityProperty> collection) : base(collection)
    {
    }
    
    public override int GetHashCode()
    {
        return this
            .Select(prop => prop.GetHashCode())
            .Aggregate(0, (current, curHash) => unchecked(current + curHash * 37));
    }
    
    public override string ToString()
    {
        return string.Join(", ", this.Select(p => p.ToString()));
    }
}

public static class PropertySetExtensions
{
    public static PropertySet ToPropertySet(this IEnumerable<IEntityProperty> props) => new(props);
    
    public static PropertySet IntoPropertySet(this IEntityProperty prop) => new([prop]);
}