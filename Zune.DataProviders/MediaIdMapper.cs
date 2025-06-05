using OwlCore.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Zune.DataProviders;

public interface IMediaIdMapper
{
    /// <summary>
    /// Finds an ID that represents the same media as the given ID but from the specified source.
    /// </summary>
    /// <param name="id">The known ID.</param>
    /// <param name="targetSource">The source to find an equivalent ID from.</param>
    /// <returns></returns>
    Task<MediaId> MapTo(MediaId id, string targetSource);
}

public interface IModifiableMediaIdMapper : IMediaIdMapper
{
    /// <summary>
    /// Registers and association between the two provided IDs.
    /// </summary>
    Task RegisterMapping(MediaId id1, MediaId id2);
}

public interface ICompleteMediaIdMapper : IMediaIdMapper
{
    /// <summary>
    /// Fetches all <see cref="MediaId"/>s associated with the given ID.
    /// </summary>
    /// <param name="id">The ID to list associations for.</param>
    IAsyncEnumerable<MediaId> EnumerateMappings(MediaId id);
}

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
