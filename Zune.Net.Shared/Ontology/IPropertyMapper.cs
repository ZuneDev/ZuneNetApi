using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using Zune.Net.Ontology.BaseProperties;
using Zune.Net.Ontology.Core;

namespace Zune.Net.Ontology;

public interface IPropertyMapper
{
    IReadOnlySet<PropertyMapping> AvailableMappings { get; }
    
    /// <summary>
    /// Maps the provided inputs to their outputs.
    /// </summary>
    Task<IPropertyBag> ExecuteAsync(IPropertyBag inputs, IReadOnlyPropertySet desiredOutputs);
}

[DebuggerDisplay("{Cost}: ({Inputs}) -> ({Outputs})")]
public record PropertyMapping(int Cost, IReadOnlyPropertySet Inputs, IReadOnlyPropertySet Outputs);

public static partial class Ep
{
    [EntityReferenceProperty(EntityFact.PrimaryImage)]
    [EntityReferenceListProperty(EntityFact.Album)]
    [EntityReferenceListProperty(EntityFact.Image)]
    [EntityReferenceListProperty(EntityFact.Influenced)]
    [EntityReferenceListProperty(EntityFact.InfluencedBy)]
    [EntityReferenceListProperty(EntityFact.SimilarTo)]
    public static partial class Artist
    {
        public static readonly EntityStringProperty Name = new(EntityType.Artist, EntityFact.Name);
        public static readonly EntityStringProperty Bio = new(EntityType.Artist, EntityFact.Bio);
    }

    [EntityReferenceProperty(EntityFact.Artist)]
    [EntityReferenceProperty(EntityFact.PrimaryImage)]
    [EntityReferenceListProperty(EntityFact.SimilarTo)]
    [EntityReferenceListProperty(EntityFact.Track)]
    [EntityReferenceListProperty(EntityFact.Genre)]
    public static partial class Album
    {
        public static readonly EntityStringProperty Name = new(EntityType.Album, EntityFact.Name);
        public static readonly ParsableTypedEntityProperty<TimeSpan> Duration = new(EntityType.Album, EntityFact.Duration);
        public static readonly ParsableTypedEntityProperty<int> TrackCount = new(EntityType.Album, EntityFact.Size1D);
    }
    
    [EntityReferenceProperty(EntityFact.Artist)]
    [EntityReferenceProperty(EntityFact.Album)]
    [EntityReferenceListProperty(EntityFact.Genre)]
    [EntityReferenceListProperty(EntityFact.SimilarTo)]
    public static partial class Track
    {
        public static readonly EntityStringProperty Name = new(EntityType.Track, EntityFact.Name);
        public static readonly ParsableTypedEntityProperty<TimeSpan> Duration = new(EntityType.Track, EntityFact.Duration);
        public static readonly ParsableTypedEntityProperty<int> Number = new(EntityType.Track, EntityFact.Number);
        public static readonly ParsableTypedEntityProperty<int> DiscNumber = new(EntityType.Track, EntityFact.DiscNumber);
    }

    [EntityReferenceListProperty(EntityFact.ImageInstance)]
    public static partial class Image
    {
    }

    public static class ImageInstance
    {
        public static readonly EntityStringProperty Url = new(EntityType.ImageInstance, EntityFact.Url);
        public static readonly TypedEntityProperty<Size> Size = new(EntityType.ImageInstance, EntityFact.Size2D);
    }
    
    [EntityReferenceListProperty(EntityFact.Album)]
    [EntityReferenceListProperty(EntityFact.Track)]
    public static partial class Genre
    {
        public static readonly EntityStringProperty Name = new(EntityType.Track, EntityFact.Name);
    }
}

public enum EntityType
{
    Unknown,
    Artist,
    Album,
    Track,
    Genre,
    Feature,
    Image,
    ImageInstance,
}

public enum EntityFact
{
    Unknown = 0,
    
    Id,
    Name,
    Bio,
    Artist,
    Album,
    Track,
    Genre,
    Influenced,
    InfluencedBy,
    SimilarTo,
    Image,
    PrimaryImage,
    ImageInstance,
    Url,
    Duration,
    Number,
    DiscNumber,
    Size1D,
    Size2D,
}