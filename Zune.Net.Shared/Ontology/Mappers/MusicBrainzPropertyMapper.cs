using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MetaBrainz.MusicBrainz;
using Zune.Net.Helpers;
using Zune.Net.Ontology.Identifiers;

namespace Zune.Net.Ontology.Mappers;

public class MusicBrainzPropertyMapper : IPropertyMapper
{
    public IReadOnlySet<PropertyMapping> AvailableMappings { get; } = GetAvailableMappings().ToHashSet();

    private static readonly Dictionary<IEntityProperty, (Guid, Func<Regex>)> ArtistPropsFromUrlRels = new()
    {
        [AllMusicIdProperty.Artist] = (MusicBrainz.UrlIdAllMusic, MusicBrainz.RxUrlAllMusicArtist),
        [DiscogsIdProperty.Artist] = (MusicBrainz.UrlIdDiscogs, MusicBrainz.RxUrlDiscogsArtist),
        [LastFmIdProperty.Artist] = (MusicBrainz.UrlIdLastFm, MusicBrainz.RxUrlLastFm),
    };

    private static readonly Dictionary<IEntityProperty, (Guid, Func<Regex>)> ReleasePropsFromUrlRels = new()
    {
        [AllMusicIdProperty.Album] = (MusicBrainz.UrlIdAllMusic, MusicBrainz.RxUrlAllMusicAlbum),
        [AllMusicIdProperty.Release] = (MusicBrainz.UrlIdAllMusic, MusicBrainz.RxUrlAllMusicRelease),
        [DiscogsIdProperty.Master] = (MusicBrainz.UrlIdDiscogs, MusicBrainz.RxUrlDiscogsMaster),
        [DiscogsIdProperty.Release] = (MusicBrainz.UrlIdDiscogs, MusicBrainz.RxUrlDiscogsRelease),
        [LastFmIdProperty.Album] = (MusicBrainz.UrlIdLastFm, MusicBrainz.RxUrlLastFm),
    };
    
    public async Task<IPropertyBag> ExecuteAsync(IPropertyBag inputs, IReadOnlyPropertySet desiredOutputs)
    {
        var outputs = new PropertyBag();
        
        try
        {
            var inputProperty = inputs.Keys.OfType<MusicBrainzIdProperty>().Single();
            var mbid = inputs.Get(inputProperty);
            
            if (inputProperty.EntityType is EntityType.Artist)
            {
                var includes = Include.None;
                
                // ---
                // I | Decide what we need to include in our request

                var releaseMbidsProp = Ep.Artist.AlbumIds(MusicBrainzIdProperty.Release);
                if (desiredOutputs.Contains(releaseMbidsProp))
                    includes |= Include.Releases;

                var releaseGroupMbidsProp = Ep.Artist.AlbumIds(MusicBrainzIdProperty.ReleaseGroup);
                if (desiredOutputs.Contains(releaseGroupMbidsProp))
                    includes |= Include.ReleaseGroups;

                var propsMappedFromRelationships = ArtistPropsFromUrlRels.Keys.Where(desiredOutputs.Contains).ToHashSet();
                if (propsMappedFromRelationships.Count > 0)
                    includes |= Include.UrlRelationships;
                
                // ---
                // II | Make the request
                
                var mbArtist = await MusicBrainz._query.LookupArtistAsync(mbid, includes);

                // ---
                // III | Parse the results
                
                outputs.Set(Ep.Artist.Name, mbArtist.Name);
                
                if (mbArtist.Releases is not null)
                {
                    var releaseMbids = mbArtist.Releases
                        .Select(r => r.Id)
                        .ToPropertyValueList(MusicBrainzIdProperty.Release);
                    outputs.Set(releaseMbidsProp, releaseMbids);
                }
                
                if (mbArtist.ReleaseGroups is not null)
                {
                    var releaseGroupMbids = mbArtist.ReleaseGroups
                        .Select(r => r.Id)
                        .ToPropertyValueList(MusicBrainzIdProperty.ReleaseGroup);
                    outputs.Set(releaseGroupMbidsProp, releaseGroupMbids);
                }

                foreach (var idProp in propsMappedFromRelationships)
                {
                    var (typeId, rxUrl) = ArtistPropsFromUrlRels[idProp];
                    
                    var relationship = mbArtist.Relationships?.FirstOrDefault(rel => rel.TypeId == typeId);
                    var resourceUrl = relationship?.Url?.Resource?.ToString();
                    if (resourceUrl is null)
                        return outputs;

                    var match = rxUrl().Match(resourceUrl);
                    if (!match.Success)
                        continue;
                    
                    var idStr = match.Groups[1].Value;
                    
                    outputs[idProp] = idStr;
                }
            }
        }
        catch (Exception e)
        {
        }

        return outputs;
    }

    private static IEnumerable<PropertyMapping> GetAvailableMappings()
    {
        const int cost = 20;
        
        yield return new PropertyMapping(cost,
            [MusicBrainzIdProperty.Artist],
            [
                Ep.Artist.Name,
                Ep.Artist.AlbumIds(MusicBrainzIdProperty.Release),
                
                ..ArtistPropsFromUrlRels.Keys,
            ]);
        
        yield return new PropertyMapping(cost,
            [MusicBrainzIdProperty.Release],
            [
                Ep.Album.Name,
                Ep.Album.ImageUrl,
                Ep.Album.ImageId(MusicBrainzIdProperty.CoverArt),
                Ep.Album.TrackIds(MusicBrainzIdProperty.Recording),
                
                ..ReleasePropsFromUrlRels.Keys,
            ]);
    }
}