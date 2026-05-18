using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public static class Artist
    {
        public static readonly EntityStringProperty Name = new(EntityType.Artist, EntityFact.Name);
        
        public static TypedEntityReferenceProperty<TId, TProvider> ImageId<TId, TProvider>(ITypedEntityIdProperty<TId, TProvider> imageProp)
            where TProvider : Enum
            => new(EntityType.Album, EntityFact.PrimaryImage, imageProp);
        
        public static TypedEntityListProperty<T> AlbumIds<T>(ITypedEntityProperty<T> childProp) => new(EntityType.Artist, EntityFact.Album, childProp);
        
        public static TypedEntityListProperty<T> InfluenceIds<T>(ITypedEntityProperty<T> childProp) => new(EntityType.Artist, EntityFact.InfluencedBy, childProp);
        
        public static TypedEntityListProperty<T> InfluencerIds<T>(ITypedEntityProperty<T> childProp) => new(EntityType.Artist, EntityFact.Influenced, childProp);
        
        public static TypedEntityListProperty<T> SimilarToIds<T>(ITypedEntityProperty<T> childProp) => new(EntityType.Artist, EntityFact.SimilarTo, childProp);
        
        public static readonly ParsableTypedEntityProperty<string> Bio = new(EntityType.Artist, EntityFact.Bio);
        
        public static readonly TypedEntityListProperty<string> PrimaryImageInstances = new(EntityType.Artist, EntityFact.PrimaryImage, ImageInstance.Url);
    }

    [EntityReferenceProperty(EntityFact.Artist)]
    [EntityReferenceProperty(EntityFact.PrimaryImage)]
    [EntityReferenceListProperty(EntityFact.Track)]
    [EntityReferenceListProperty(EntityFact.SimilarTo)]
    public static partial class Album
    {
        public static readonly EntityStringProperty Name = new(EntityType.Album, EntityFact.Name);
        
        public static TypedEntityListProperty<T> TrackIds<T>(ITypedEntityProperty<T> childProp) => new(EntityType.Album, EntityFact.Track, childProp);
        public static EntityListProperty TrackIds(IEntityProperty childProp) => new(EntityType.Album, EntityFact.Track, childProp);
        
        public static TypedEntityListProperty<T> SimilarToIds<T>(ITypedEntityProperty<T> childProp) => new(EntityType.Album, EntityFact.SimilarTo, childProp);
    }

    public static class ImageInstance
    {
        public static readonly EntityStringProperty Url = new(EntityType.Image, EntityFact.Url);
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
    Influenced,
    InfluencedBy,
    SimilarTo,
    Image,
    PrimaryImage,
    ImageInstance,
    Url,
    Size2D,
    Duration,
    Number,
    DiscNumber
}