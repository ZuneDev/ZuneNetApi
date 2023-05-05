using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zune.Net.MetaServices.DomainModels.Endpoints
{
    [XmlRoot("METADATA")]
    public class Metadata
    {
        public Metadata()
        {
            var baseUrl = "http://metaservices.zune.net/ZuneFAI/";
            var endpointList = new List<string>()
            {
                "Search", "GetAlbumDetailsByToc", "SubmitMatchFeedback", "GetAlbumDetailsFromToc",
                "GetAlbumDetails", "GetAlbumLiteByTrackWMname", "GetAlbumLiteByToc", "GetAlbumDetailsFromAlbumId",
                "GetAlbumDetailsByAlbumId", "SubmitEditFeedback", "SubmitAddFeedback", "GetResultsForArtist"
            };

            var _endpoints = new List<Endpoint>();

            foreach(var endpoint in endpointList)
            {
                _endpoints.Add(new Endpoint()
                {
                    Name = endpoint,
                    Uri = $"{baseUrl}{endpoint}"
                });
            }

            endpoints = new Endpoints()
            {
                endpoints = _endpoints
            };
        }
        [XmlElement("ENDPOINTS")]
        public Endpoints endpoints;
    }
}