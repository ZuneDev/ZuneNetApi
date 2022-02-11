using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;
using Zune.Xml.Catalog;

namespace Zune.Net.Catalog.Helpers
{
    public partial class MusicBrainz
    {
        public Genre GetGenreByMBID(Guid mbid)
        {

        }

        public Genre MBGenreToGenre(IGenre mb_genre)
        {
            return new()
            {
                Id = mb_genre.Id.ToString(),
                Title = mb_genre.Name
            };
        }
    }
}
