using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.MDSR
{
    public class Result
    {

        [XmlElement]
        public bool bestmatch { get; set; } = false;

        [XmlElement]
        public Int64 album_id { get; set; } = 0;
        [XmlElement]
        public int Volume { get; set; } = 0;

        [XmlElement]
        public string albumPerformer { get; set; } = string.Empty; // Album Artist

        [XmlElement]
        public string albumCover { get; set; } = string.Empty; // probably a URL. Does nothing so far
        [XmlElement]
        public string buyNowLink { get; set; } = string.Empty; // redirects to http://social.zune.net/album/FindAlbum.aspx?<value_here>&culture=en-US
  
        [XmlElement]
        public string albumReleaseDate { get; set; } = string.Empty; // yyyy-mm-dd
 
        [XmlElement]
        public string albumGenre { get; set; } = string.Empty; // i.e. dance
  
        [XmlElement]
        public bool IsMultiDisk { get; set; } = false;
        [XmlElement]
        public int numberOfTracks {get; set;} = 0;
    }
}