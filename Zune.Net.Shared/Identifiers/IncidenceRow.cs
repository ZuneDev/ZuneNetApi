using System.Collections.Generic;

namespace Zune.Net.Identifiers;

internal record IncidenceRow(HashSet<IPropertyMapper> InputTo, HashSet<IPropertyMapper> OutputFrom)
{
    public IncidenceRow() : this(new(), new())
    {
    }
}

internal class IncidenceMatrix : Dictionary<EntityProperty, IncidenceRow>;