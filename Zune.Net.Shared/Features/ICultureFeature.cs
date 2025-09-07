#nullable enable

using System;
using System.Globalization;

namespace Zune.Net.Features;

public interface ICultureFeature
{
    string CultureString { get; }

    string? Region { get; }
    
    string Language { get; }
}

public record CultureFeature : ICultureFeature
{
    public CultureFeature(string cultureString)
    {
        CultureString = cultureString;
        
        var parts = cultureString.Split('-');
        if (parts.Length < 1)
            throw new ArgumentException("Expected non-empty culture string");
        
        Language = parts[0];
        if (parts.Length >= 2)
            Region = parts[1];
    }
    
    public string Language { get; }

    public string? Region { get; }
    
    public string CultureString { get; }
}

public static class ICultureFeatureExtensions
{
    public static CultureInfo GetCultureInfo(this ICultureFeature feature)
    {
        return new CultureInfo(feature.CultureString);
    }
}
