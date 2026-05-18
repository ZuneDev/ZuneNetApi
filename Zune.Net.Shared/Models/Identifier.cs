using System;

namespace Zune.Net.Models;

public interface IIdentifier<out TValue, out TProviderEntities>
{
    TValue Value { get; }
    TProviderEntities ProviderEntityType { get; }
}

public record Identifier<TValue, TProviderEntities>(TValue Value, TProviderEntities ProviderEntityType)
    : IIdentifier<TValue, TProviderEntities>;

public interface IHasIdentifier<out TId, out TIdValue, out TProviderEntities>
    where TId : IIdentifier<TIdValue, TProviderEntities>
{
    TId Id { get; }
}