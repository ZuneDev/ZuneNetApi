namespace Zune.Net.Models;

public record Image<TId, TIdValue, TProviderEntities>(TId Id, string Url)
    : IHasIdentifier<TId, TIdValue, TProviderEntities>
    where TId : IIdentifier<TIdValue, TProviderEntities>;

public record ImageInstance<TId, TIdValue, TProviderEntities>(TId Id, string Url, string Format, int Width, int Height)
    : IHasIdentifier<TId, TIdValue, TProviderEntities>
    where TId : IIdentifier<TIdValue, TProviderEntities>;