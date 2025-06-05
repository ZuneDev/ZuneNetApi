using OwlCore.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zune.DataProviders;

public class CascadedMediaIdMapper : IMediaIdMapper
{
    public List<IMediaIdMapper> Mappers { get; } = [];

    public async Task<MediaId> MapTo(MediaId id, string targetSource)
    {
        if (id.Source.OrdinalEquals(targetSource))
            return id;

        foreach (var mapper in Mappers)
        {
            var mappedId = await mapper.MapTo(id, targetSource);
            if (mappedId is not null)
                return mappedId;
        }

        return null;
    }
}

public class MemoryCachedMediaIdMapper(IMediaIdMapper innerMapper) : IMediaIdMapper, IDelegatable<IMediaIdMapper>
{
    private readonly Dictionary<MediaId, MediaId> _cache = [];

    public IMediaIdMapper Inner { get; } = innerMapper;

    public async Task<MediaId> MapTo(MediaId id, string targetSource)
    {
        if (id.Source.OrdinalEquals(targetSource))
            return id;

        if (_cache.TryGetValue(id, out var mappedId))
            return mappedId;

        mappedId = await Inner.MapTo(id, targetSource);

        _cache[id] = mappedId;
        _cache[mappedId] = id;

        return mappedId;
    }
}
