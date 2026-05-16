using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Zune.Net.Ontology;

[CollectionBuilder(typeof(IReadOnlyPropertySetBuilder), nameof(IReadOnlyPropertySetBuilder.Create))]
public interface IReadOnlyPropertySet : IReadOnlySet<IEntityProperty>;

[CollectionBuilder(typeof(IPropertySetBuilder), nameof(IPropertySetBuilder.Create))]
public interface IPropertySet : IReadOnlyPropertySet, ISet<IEntityProperty>;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class PropertySet : HashSet<IEntityProperty>, IPropertySet
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
            .Select(prop => EqualityComparer<IEntityProperty>.Default.GetHashCode(prop))
            .Aggregate(0, (current, curHash) => unchecked(current + curHash * 37));
    }
    
    public override string ToString()
    {
        return string.Join(", ", this.Select(p => p.ToString()));
    }
}

public static class PropertySetExtensions
{
    public static IPropertySet ToPropertySet(this IEnumerable<IEntityProperty> props) => new PropertySet(props);
    
    public static IPropertySet IntoPropertySet(this IEntityProperty prop) => new PropertySet([prop]);
}