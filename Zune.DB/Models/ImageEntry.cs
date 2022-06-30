using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Zune.DB.Models
{
    public class ImageEntry
    {
        public ImageEntry()
        {

        }

        public ImageEntry(Guid id, string url)
        {
            Id = id;
            Url = url;
        }

        [BsonId]
        public Guid Id { get; set; }

        public string Url { get; set; }
    }
}
