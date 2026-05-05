using System.Collections.Generic;
using System.Linq;

namespace Zune.Net.Identifiers;

public interface IReadOnlyPropertySet : IReadOnlySet<EntityProperty>;

public interface IPropertySet : IReadOnlyPropertySet, ISet<EntityProperty>;

public class PropertySet : HashSet<EntityProperty>, IPropertySet
{
    public PropertySet(IEnumerable<EntityProperty> collection) : base(collection)
    {
    }
    
    public override int GetHashCode()
    {
        return this
            .Select(prop => EqualityComparer<EntityProperty>.Default.GetHashCode(prop))
            .Aggregate(0, (current, curHash) => unchecked(current + curHash * 37));
    }
}

public static class PropertySetExtensions
{
    public static IPropertySet ToPropertySet(this IEnumerable<EntityProperty> props) => new PropertySet(props);
}