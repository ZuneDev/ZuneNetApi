namespace AppleMusicSharp;

internal static class Constants
{
    public const string AM_TOKEN = "eyJhbGciOiJFUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6IldlYlBsYXlLaWQifQ.eyJpc3MiOiJBTVBXZWJQbGF5IiwiaWF0IjoxNzU2ODM5MzgzLCJleHAiOjE3NjQwOTY5ODMsInJvb3RfaHR0cHNfb3JpZ2luIjpbImFwcGxlLmNvbSJdfQ.NLkNKypRCkEIzuRgU7n6t4G0yS87uQusV0TsxGHochj6Qz92rCuJ93bI2kuWRBDCiGC9LrDCR4dv395hZU5-xw";
    public const string AM_ORIGIN = "https://music.apple.com";
    public const string BASEURL_AMPAPIEDGE = "https://amp-api-edge.music.apple.com";
    private const string BASEURL_API = "https://api.music.apple.com";
}

public class ResizeMode
{
    public const string None = "";
    public const string BoundingBox = "bb";
    public const string BackgroundFill = "bf";
    public const string PreserveWidth = "w";
    public const string PreserveHeight = "h";
    public const string ScaleCrop = "sr";
}