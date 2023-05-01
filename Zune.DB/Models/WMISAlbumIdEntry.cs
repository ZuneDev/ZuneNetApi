using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Zune.DB.Models
{
    public class WMISAlbumIdEntry
    {
        [BsonId]
        public string Id;
        public Int64 AlbumId {get; set;}
        public Guid AlbumGuid {get; set;}

        public WMISAlbumIdEntry(Int64 id, Guid guid)
        {
            Id = $"{guid.ToString()}-{id}";
            AlbumId = id;
            AlbumGuid = guid;
        }
    }
}