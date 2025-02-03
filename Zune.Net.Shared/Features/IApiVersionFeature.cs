namespace Zune.Net.Features;

public interface IApiVersionFeature
{
    string Version { get; }
}

public record ApiVersionFeature(string Version) : IApiVersionFeature;
