using System.Globalization;

namespace Zune.Net.Features;

public interface ICultureFeature
{
    string CultureString { get; }

    CultureInfo GetCultureInfo();
}

public record CultureFeature(string CultureString) : ICultureFeature
{
    public CultureInfo GetCultureInfo() => new(CultureString);
}
