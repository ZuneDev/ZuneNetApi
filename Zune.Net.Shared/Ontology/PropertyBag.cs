using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Zune.Net.Ontology;

public interface IPropertyBag : IDictionary<IEntityProperty, object>
{
    IReadOnlyPropertySet AsReadOnlyPropertySet();

    bool TryGetForSet(IReadOnlyPropertySet properties, out IPropertyBag bag);
}

[DebuggerDisplay("{DebuggerDisplay,nq}")]
public class PropertyBag(IDictionary<IEntityProperty, object> properties = null) : IPropertyBag
{
    private readonly IDictionary<IEntityProperty, object> _properties = properties
        ?? new Dictionary<IEntityProperty, object>();

    private string DebuggerDisplay => ToString();

    #region IDictionary implementation

    public IEnumerator<KeyValuePair<IEntityProperty, object>> GetEnumerator() => _properties.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_properties).GetEnumerator();

    public void Add(KeyValuePair<IEntityProperty, object> item) =>
        ((ICollection<KeyValuePair<IEntityProperty, object>>)_properties).Add(item);

    public void Clear() => _properties.Clear();

    public bool Contains(KeyValuePair<IEntityProperty, object> item) =>
        ((ICollection<KeyValuePair<IEntityProperty, object>>)_properties).Contains(item);

    public void CopyTo(KeyValuePair<IEntityProperty, object>[] array, int arrayIndex) =>
        ((ICollection<KeyValuePair<IEntityProperty, object>>)_properties).CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<IEntityProperty, object> item) =>
        ((ICollection<KeyValuePair<IEntityProperty, object>>)_properties).Remove(item);

    public int Count => _properties.Count;
    public bool IsReadOnly => false;

    public void Add(IEntityProperty key, object value) => _properties.Add(key, value);

    public bool ContainsKey(IEntityProperty key) => _properties.ContainsKey(key);

    public bool Remove(IEntityProperty key) => _properties.Remove(key);

    public bool TryGetValue(IEntityProperty key, [MaybeNullWhen(false)] out object value) =>
        _properties.TryGetValue(key, out value);

    public object this[IEntityProperty key]
    {
        get => _properties[key];
        set => _properties[key] = value;
    }

    public ICollection<IEntityProperty> Keys => _properties.Keys;
    public ICollection<object> Values => _properties.Values;

    #endregion

    public IReadOnlyPropertySet AsReadOnlyPropertySet() => new PropertySetView(_properties.Keys);
    
    public bool TryGetForSet(IReadOnlyPropertySet properties, out IPropertyBag bag)
    {
        bag = new PropertyBag();
        foreach (var property in properties)
        {
            if (!_properties.TryGetValue(property, out var value))
                return false;
            
            bag.Add(property, value);
        }

        return true;
    }
    
    public override int GetHashCode()
    {
        return _properties.Keys
            .Select(prop => EqualityComparer<IEntityProperty>.Default.GetHashCode(prop))
            .Aggregate(0, (current, curHash) => unchecked(current + curHash * 37));
    }

    public override string ToString()
    {
        return string.Join(", ", _properties.Keys.Select(p => p.ToString()));
    }

    private class PropertySetView(ICollection<IEntityProperty> keys) : IReadOnlyPropertySet
    {
        public int Count => keys.Count;

        public bool Contains(IEntityProperty item) => keys.Contains(item);

        public IEnumerator<IEntityProperty> GetEnumerator() => keys.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => keys.GetEnumerator();

        public bool IsProperSubsetOf(IEnumerable<IEntityProperty> other) => keys.ToHashSet().IsProperSubsetOf(other);
        public bool IsProperSupersetOf(IEnumerable<IEntityProperty> other) => keys.ToHashSet().IsProperSupersetOf(other);
        public bool IsSubsetOf(IEnumerable<IEntityProperty> other) => keys.ToHashSet().IsSubsetOf(other);
        public bool IsSupersetOf(IEnumerable<IEntityProperty> other) => keys.ToHashSet().IsSupersetOf(other);
        public bool Overlaps(IEnumerable<IEntityProperty> other) => keys.Any(other.Contains);
        public bool SetEquals(IEnumerable<IEntityProperty> other) => keys.ToHashSet().SetEquals(other);
    }
}

public static class PropertyBagExtensions
{
    public static IPropertyBag ToPropertyBag(this IDictionary<IEntityProperty, object> values)
    {
        return new PropertyBag(values);
    }

    public static void SetFrom(this IPropertyBag target, IPropertyBag source)
    {
        foreach (var (prop, value) in source)
            target[prop] = value;
    }
    
    public static void TryAddFrom(this IPropertyBag target, IPropertyBag source)
    {
        foreach (var (prop, value) in source)
            target.TryAdd(prop, value);
    }
    
    public static T Get<T>(this IPropertyBag bag, TypedEntityProperty<T> prop) => (T)bag[prop];
    
    public static IEnumerable<T> Get<T>(this IPropertyBag bag, TypedListEntityProperty<T> prop) => (IEnumerable<T>)bag[prop];
}