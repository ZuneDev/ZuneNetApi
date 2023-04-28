using System;
using System.Collections.Generic;

namespace Zune.Xml.MDAR
{
    public class AlbumDetails
    {
        public string Title;
        public Int64 AlbumId;
        public int Volume;
        public List<AlbumDetailsTrack> Items;
        public string ReturnCode;
    }
}