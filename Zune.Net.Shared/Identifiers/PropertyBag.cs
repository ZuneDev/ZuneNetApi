using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Zune.Net.Identifiers;

public interface IPropertyBag : IDictionary<EntityProperty, object>
{
    IReadOnlyPropertySet AsReadOnlyPropertySet();
    
    IPropertyBag GetForSet(IReadOnlyPropertySet properties);
}

public class PropertyBag(IDictionary<EntityProperty, object> properties = null) : IPropertyBag
{
    private readonly IDictionary<EntityProperty, object> _properties = properties
        ?? new Dictionary<EntityProperty, object>();

    #region IDictionary implementation

    public IEnumerator<KeyValuePair<EntityProperty, object>> GetEnumerator() => _properties.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_properties).GetEnumerator();

    public void Add(KeyValuePair<EntityProperty, object> item) =>
        ((ICollection<KeyValuePair<EntityProperty, object>>)_properties).Add(item);

    public void Clear() => _properties.Clear();

    public bool Contains(KeyValuePair<EntityProperty, object> item) =>
        ((ICollection<KeyValuePair<EntityProperty, object>>)_properties).Contains(item);

    public void CopyTo(KeyValuePair<EntityProperty, object>[] array, int arrayIndex) =>
        ((ICollection<KeyValuePair<EntityProperty, object>>)_properties).CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<EntityProperty, object> item) =>
        ((ICollection<KeyValuePair<EntityProperty, object>>)_properties).Remove(item);

    public int Count => _properties.Count;
    public bool IsReadOnly => false;

    public void Add(EntityProperty key, object value) => _properties.Add(key, value);

    public bool ContainsKey(EntityProperty key) => _properties.ContainsKey(key);

    public bool Remove(EntityProperty key) => _properties.Remove(key);

    public bool TryGetValue(EntityProperty key, [MaybeNullWhen(false)] out object value) =>
        _properties.TryGetValue(key, out value);

    public object this[EntityProperty key]
    {
        get => _properties[key];
        set => _properties[key] = value;
    }

    public ICollection<EntityProperty> Keys => _properties.Keys;
    public ICollection<object> Values => _properties.Values;

    #endregion
    
    public IReadOnlyPropertySet AsReadOnlyPropertySet() => new PropertySetView(_properties.Keys);
    
    public IPropertyBag GetForSet(IReadOnlyPropertySet properties)
    {
        var bag = new PropertyBag();
        foreach (var property in properties)
        {
            if (_properties.TryGetValue(property, out var value))
                bag.Add(property, value);
        }
        return bag;
    }
    
    public override int GetHashCode()
    {
        return _properties.Keys
            .Select(prop => EqualityComparer<EntityProperty>.Default.GetHashCode(prop))
            .Aggregate(0, (current, curHash) => unchecked(current + curHash * 37));
    }

    public override string ToString()
    {
        return string.Join("; ", _properties.Keys.Select(p => p.ToString()));
    }

    private class PropertySetView(ICollection<EntityProperty> keys) : IReadOnlyPropertySet
    {
        public int Count => keys.Count;

        public bool Contains(EntityProperty item) => keys.Contains(item);

        public IEnumerator<EntityProperty> GetEnumerator() => keys.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => keys.GetEnumerator();

        public bool IsProperSubsetOf(IEnumerable<EntityProperty> other) => keys.ToHashSet().IsProperSubsetOf(other);
        public bool IsProperSupersetOf(IEnumerable<EntityProperty> other) => keys.ToHashSet().IsProperSupersetOf(other);
        public bool IsSubsetOf(IEnumerable<EntityProperty> other) => keys.ToHashSet().IsSubsetOf(other);
        public bool IsSupersetOf(IEnumerable<EntityProperty> other) => keys.ToHashSet().IsSupersetOf(other);
        public bool Overlaps(IEnumerable<EntityProperty> other) => keys.Any(other.Contains);
        public bool SetEquals(IEnumerable<EntityProperty> other) => keys.ToHashSet().SetEquals(other);
    }
}

public static class PropertyBagExtensions
{
    public static IPropertyBag ToPropertyBag(this IDictionary<EntityProperty, object> values)
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
}