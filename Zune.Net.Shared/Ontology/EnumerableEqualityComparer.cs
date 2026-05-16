using System.Collections.Generic;
using System.Linq;

namespace Zune.Net.Ontology;

public class EnumerableEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
{
    public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
    {
        if (x is null != y is null)
            return false;
        
        return ReferenceEquals(x, y) || x.SequenceEqual(y);
    }

    public int GetHashCode(IEnumerable<T> enumerable)
    {
        unchecked
        {
            return enumerable.Aggregate(19, (current, item) => current * 31 + item.GetHashCode());
        }
    }
}