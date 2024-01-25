using Atom.Xml;
using MetaBrainz.MusicBrainz.Interfaces;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using MetaBrainz.MusicBrainz.Interfaces.Searches;
using System;
using System.Collections.Generic;
using System.Linq;
using Zune.Xml.Catalog;

namespace Zune.Net.Helpers
{
    public partial class MusicBrainz
    {
        public static Feed<Genre> GetGenres(string requestPath)
        {
            return new()
            {
                Id = "genres",
                Title = "Genres",
                Updated = GenresLastUpdated,
                Links = { new(requestPath) },
                Entries = Genres.Keys.ToList()
            };
        }

        public static Genre GetGenreByMBID(Guid mbid)
        {
            var mb_genre = _query.LookupGenre(mbid);
            return MBGenreToGenre(mb_genre);
        }

        public static Genre GetGenreByZID(string zid)
        {
            return Genres.Select(g => g.Key).First(g => g.Id == zid);
        }

        public static Feed<Album> GetGenreAlbumsByMBID(Guid mbid, string requestPath)
        {
            var mb_genre = _query.LookupGenre(mbid);
            var updated = DateTime.Now;
            Feed<Album> feed = new()
            {
                Id = mbid.ToString(),
                Title = mb_genre.Name,
                Links = { new(requestPath) },
                Updated = updated,
                Entries = new()
            };

            // Get albums from genre
            int maxNumAlbums = 50;
            IEnumerator<ISearchResult<IReleaseGroup>> results = _query.FindAllReleaseGroups($"tag:\"{mb_genre.Name}\"").GetEnumerator();

            // Add results to feed
            while (feed.Entries.Count < maxNumAlbums && results.MoveNext())
            {
                var mb_release_grp = results.Current.Item;
                feed.Entries.Add(MBReleaseGroupToAlbum(mb_release_grp, updated: updated));
            }

            return feed;
        }

        public static Feed<Album> GetGenreAlbumsByZID(string zid, string requestPath)
        {
            var genre = Genres.First(g => g.Key.Id == zid);
            var updated = DateTime.Now;
            Feed<Album> feed = new()
            {
                Id = zid,
                Title = genre.Key.Title,
                Links = { new(requestPath) },
                Updated = updated,
                Entries = new()
            };

            // Get albums from subgenres
            int maxNumAlbums = 50;
            var mb_genres = genre.Value.Values.GetEnumerator();
            while (feed.Entries.Count < maxNumAlbums && mb_genres.MoveNext())
            {
                IEnumerator<ISearchResult<IReleaseGroup>> results = _query.FindAllReleaseGroups($"tag:\"{mb_genres.Current}\"").GetEnumerator();

                // Add results to feed
                while (feed.Entries.Count < maxNumAlbums && results.MoveNext())
                {
                    IReleaseGroup mb_release_grp = results.Current.Item;
                    feed.Entries.Add(MBReleaseGroupToAlbum(mb_release_grp, updated: updated));
                }
            }

            return feed;
        }

        public static Feed<Artist> GetGenreArtistsByMBID(Guid mbid, string requestPath)
        {
            var mb_genre = _query.LookupGenre(mbid);
            var updated = DateTime.Now;
            Feed<Artist> feed = new()
            {
                Id = mbid.ToString(),
                Title = mb_genre.Name,
                Links = { new(requestPath) },
                Updated = updated,
                Entries = ((IEnumerable<ISearchResult<IArtist>>)_query.FindAllArtists($"tag:\"{mb_genre.Name}\""))
                                .Select(mb_artist => MBArtistToArtist(mb_artist.Item, updated: updated)).ToList()
            };

            return feed;
        }

        public static Feed<Artist> GetGenreArtistsByZID(string zid, string requestPath)
        {
            var genre = Genres.First(g => g.Key.Id == zid);
            var updated = DateTime.Now;
            Feed<Artist> feed = new()
            {
                Id = zid,
                Title = genre.Key.Title,
                Links = { new(requestPath) },
                Updated = updated,
                Entries = new()
            };

            // Get artists from subgenres
            int maxNumArtists = 150;
            var mb_genres = genre.Value.Values.GetEnumerator();
            while (feed.Entries.Count < maxNumArtists && mb_genres.MoveNext())
            {
                var results = _query.FindAllArtists($"tag:\"{mb_genres.Current}\"").GetEnumerator();

                // Add results to feed
                while (feed.Entries.Count < maxNumArtists && results.MoveNext())
                {
                    IArtist mb_artist = results.Current.Item;
                    feed.Entries.Add(MBArtistToArtist(mb_artist, updated: updated));
                }
            }

            return feed;
        }

        public static Feed<Track> GetGenreTracksByMBID(Guid mbid, string requestPath)
        {
            var mb_genre = _query.LookupGenre(mbid);
            var updated = DateTime.Now;
            Feed<Track> feed = new()
            {
                Id = mbid.ToString(),
                Title = mb_genre.Name,
                Links = { new(requestPath) },
                Updated = updated,
                Entries = ((IEnumerable<ISearchResult<IRecording>>)_query.FindAllRecordings($"tag:\"{mb_genre.Name}\""))
                                .Select(mb_track => MBRecordingToTrack(mb_track.Item, updated: updated)).ToList()
            };

            return feed;
        }

        public static Feed<Track> GetGenreTracksByZID(string zid, string requestPath)
        {
            var genre = Genres.First(g => g.Key.Id == zid);
            var updated = DateTime.Now;
            Feed<Track> feed = new()
            {
                Id = zid,
                Title = genre.Key.Title,
                Links = { new(requestPath) },
                Updated = updated,
                Entries = new()
            };

            // Get tracks from subgenres
            int maxNumTracks = 50;
            var mb_genres = genre.Value.Values.GetEnumerator();
            while (feed.Entries.Count < maxNumTracks && mb_genres.MoveNext())
            {
                IEnumerator<ISearchResult<IRecording>> results = _query.FindAllRecordings($"tag:\"{mb_genres.Current}\"").GetEnumerator();

                // Add results to feed
                while (feed.Entries.Count < maxNumTracks && results.MoveNext())
                {
                    var mb_recording = results.Current.Item;
                    feed.Entries.Add(MBRecordingToTrack(mb_recording, updated: updated));
                }
            }

            return feed;
        }


        public static Genre MBGenreToGenre(IGenre mb_genre)
        {
            return new(mb_genre.Id.ToString(), mb_genre.Name);
        }


        public static readonly DateTime GenresLastUpdated = new(2021, 2, 21);

        public static readonly Dictionary<Genre, Dictionary<Guid, string>> Genres = new()
        {
            [new("rock", "Rock")] = new()
            {
                { new("ef063b60-3206-466c-a7d8-7d317fcdf9fc"), "acid rock" },
                { new("00055e8b-b951-46e2-af1e-58b5624e7952"), "acoustic rock" },
                { new("aca1caab-09d1-44b5-ae84-590ff1d218fc"), "aor" },
                { new("7983ff25-ddf9-411e-a7f9-6cca238bff79"), "alternative metal" },
                { new("e096efc6-6c9c-4240-b627-d7bbb5e8fa2a"), "alternative punk" },
                { new("ceeaa283-5d7b-4202-8d1d-e25d116b2a18"), "alternative rock" },
                { new("e12fe025-4c13-485e-a2fe-ad5bf2cd75eb"), "anarcho-punk" },
                { new("26af3760-750a-4b5e-a699-78c87697585a"), "arena rock" },
                { new("6e105daf-c06d-431e-8e33-1b8da3e48f7e"), "art punk" },
                { new("b7ef058e-6d83-4ca4-8123-9724bff4648b"), "art rock" },
                { new("d14a4c4a-bea5-4b08-b945-59b048acd2d5"), "atmospheric black metal" },
                { new("77c0160b-8090-4493-b229-890addfee1df"), "avant-garde metal" },
                { new("e5abef91-49eb-491c-b0ce-c6dd4e1f2d19"), "avant-prog" },
                { new("12568698-97cc-49a9-80ab-9c5e3fe4dd33"), "black metal" },
                { new("f976d48b-965e-4fe9-aaa6-105aab988c7c"), "blackened death metal" },
                { new("a687ea57-487a-440d-b5fd-2f1b2f8b1f57"), "blackgaze" },
                { new("58e325d5-54fd-4e98-b39a-3aa6bc319273"), "blues rock" },
                { new("f07bf7f4-1b55-42ea-8c4d-9906fce84cdf"), "boogie rock" },
                { new("12d11d01-cecc-4f9b-8d3a-99e4590952fe"), "brutal death metal" },
                { new("9c2f8b08-9bac-4bfd-b9ed-6381c65791e9"), "canterbury scene" },
                { new("36c0f210-b6c1-4481-bc7c-33de031a4046"), "celtic punk" },
                { new("f0843e91-c025-4dd7-8aef-6fe3d35470d9"), "chamber pop" },
                { new("5fef47af-772b-48e2-8e49-855f7d999e81"), "city pop" },
                { new("93244085-20e5-4f16-9067-1d19143b3810"), "classic rock" },
                { new("8b945b77-ccf8-4e55-a9a5-103cca5c1c96"), "comedy rock" },
                { new("fb07b1e3-411d-4146-a0d6-3479c0e7557b"), "country rock" },
                { new("ac4806f4-b2a8-43e8-8014-4f2bd9d64dad"), "cowpunk" },
                { new("eee90717-3eee-41bd-b463-003c180544bd"), "crossover prog" },
                { new("f66d7266-eb3d-4ef3-b4d8-b7cd992f918b"), "crust punk" },
                { new("ad41288f-0803-4e2e-859f-148e7f7b2eae"), "d-beat" },
                { new("509c006f-67c2-4c54-a568-d2b415cb0642"), "dance-punk" },
                { new("c4a72fcd-e291-43c8-a546-9ce1764ff31b"), "dark wave" },
                { new("eacfa027-2fad-413f-a2f1-80fa43674f0b"), "death metal" },
                { new("2bd1ff1a-777d-4c5c-a1cb-92792adcf10c"), "death-doom metal" },
                { new("d39cbc30-7dc6-4194-8f99-6d35dd602451"), "deathcore" },
                { new("619c5945-6484-4a68-96e9-a48d4a4e1f73"), "deathgrind" },
                { new("d5e21e4f-f6fe-464d-b269-14cf670a6949"), "deathrock" },
                { new("8b6f2489-4f8a-4bff-80d6-9f39ef672493"), "depressive black metal" },
                { new("bc1b8c46-5a69-4262-b909-c376f5de491a"), "desert rock" },
                { new("2c63c5f4-280d-466c-bcd1-7905a4795d5b"), "djent" },
                { new("db815815-60fe-44a3-9859-2232df479dc9"), "doom metal" },
                { new("6242615b-5620-465b-b6fb-2e593e90cbad"), "dream pop" },
                { new("c4a69842-f891-4569-9506-1882aa5db433"), "electronic rock" },
                { new("c187da0e-48a0-4de5-b759-4ca2a1503008"), "electropunk" },
                { new("ef0c2769-9c8f-49fc-b66e-27b8242b004a"), "emo" },
                { new("17466ce1-b750-4f75-9341-c68b16531ba3"), "emo pop" },
                { new("1d6489a9-5837-43c9-8155-94a126e8a951"), "emocore" },
                { new("65c97e89-b42b-45c2-a70e-0eca1b8f0ff7"), "experimental rock" },
                { new("f5b0242a-ede0-4929-9361-4eddeb0516b5"), "folk metal" },
                { new("6cec5b4d-3c64-44a6-914d-3e833432f631"), "folk punk" },
                { new("445dc48f-5233-4b4f-987f-867ff9ade836"), "folk rock" },
                { new("0caaa092-27c4-413e-841c-7c4deeec57a7"), "funk metal" },
                { new("eca476ac-d63b-431f-9347-c45f530dc15d"), "funk rock" },
                { new("fd5bea65-15ef-44c4-83fb-ba15013b3596"), "garage punk" },
                { new("b7bb11c3-bb83-4047-af65-1e8aba89039f"), "garage rock" },
                { new("bc08993e-b188-43e8-b358-b84f2db6bfe6"), "glam" },
                { new("818134e3-9392-4624-a13c-c4b19c54bccd"), "glam metal" },
                { new("54d89e62-5bfb-42bc-a321-9230e6fdcd75"), "glam rock" },
                { new("ca315be3-733b-4630-92d9-cd8ecafe0419"), "goregrind" },
                { new("d9f42dc1-3a92-4217-8ae5-e9672f595b8e"), "gothic" },
                { new("c73392a9-adaf-4ee4-b911-97a5892ac917"), "gothic metal" },
                { new("d9858aef-25bf-478b-b135-b900aa1f0fd1"), "gothic rock" },
                { new("b9c9a69d-5fc2-4317-b25d-5a96c8c9478a"), "grebo" },
                { new("093149ea-d1bc-4f93-bd5a-8fdcd055ea2f"), "grindcore" },
                { new("1a85800e-945e-4a9d-824e-dbd21ca15757"), "groove metal" },
                { new("77f8d0ee-059a-4420-9ac7-84890730ae2a"), "group sounds" },
                { new("3f08faa9-6c3e-4b6d-b4b8-08eea45954e2"), "grunge" },
                { new("4ab0dfbd-1c0b-45e6-8cb8-3acf78a3bd77"), "gypsy punk" },
                { new("51cb9f91-e6a2-41bf-891f-e78e3f1e52ab"), "hard rock" },
                { new("055a6e4d-d929-42ac-ba7a-9d063b254ea5"), "hardcore punk" },
                { new("a6719055-99c4-47eb-beaa-71081f2376f9"), "heavy metal" },
                { new("3bc3d7dd-2bc5-4848-a9ef-4b67c6f7c6f9"), "heavy psych" },
                { new("80778e31-3eca-4391-b01d-68c0e1cf40f5"), "heavy rock" },
                { new("5fdbe79f-e728-4caa-81d3-669f1c6e4de2"), "hopepunk" },
                { new("36471baf-3fd6-459d-918c-40dc64adea36"), "horror punk" },
                { new("ccd19ebf-052c-4afe-8ad9-fbb0a73f94a7"), "indie rock" },
                { new("fde4b353-2d97-4b57-af38-e7c53efa84f3"), "indorock" },
                { new("d4df54b5-67b4-4fb7-8f73-79e71717a501"), "industrial metal" },
                { new("ffbc9907-c9be-4ace-876b-b7fd5b9d51f9"), "industrial rock" },
                { new("6b3903aa-8abe-4645-b363-558483b1586f"), "instrumental rock" },
                { new("d181e978-c908-4f33-b2a2-f7454d0cb2c1"), "j-rock" },
                { new("5eea976c-1ab2-4d94-9b63-7d1297d5dba5"), "jazz rock" },
                { new("5319fada-431d-4b16-ac6f-37e24b2201b5"), "kawaii metal" },
                { new("ba318056-9ddf-46cd-8b95-61fc993b962d"), "krautrock" },
                { new("b07b5eec-c7f0-4529-a876-e0dd6b350084"), "latin rock" },
                { new("caec8f8d-d862-4b62-9942-5622c2c18446"), "mainstream rock" },
                { new("a6b55d73-bba3-49fc-a361-ca4bde004717"), "math rock" },
                { new("8df1241b-2ba1-40b8-bb8f-2dc15ce6dc1e"), "mathcore" },
                { new("74ed98e0-d2f4-4d34-9a51-0c18c1303726"), "melodic black metal" },
                { new("5eeb0431-8160-40a7-ae44-c2ad01fed156"), "melodic death metal" },
                { new("4f2cd717-7259-48df-8d0a-c17ccd9203b9"), "melodic metalcore" },
                { new("e6d5e340-d969-4bb1-8342-18abc1d8c611"), "melodic rock" },
                { new("cc28f5aa-5e19-487c-88e1-e56b022f1fdd"), "metal" },
                { new("62f88295-0605-4ce8-958b-2e0063ece47e"), "metalcore" },
                { new("d024e2e6-842c-43f7-b46e-8d0b87a4cd8e"), "mincecore" },
                { new("d2327987-362f-4d70-8bfd-a8b47c3b0a54"), "neo-progressive rock" },
                { new("cd99451c-f6d5-47c7-8d49-6c08b51e61aa"), "neo-psychedelia" },
                { new("66366e89-590e-424f-b13d-0d0564eb4f27"), "neo-rockabilly" },
                { new("28f8cf28-d1d6-4df9-8bee-ab0c6a9f0721"), "neue deutsche härte" },
                { new("3ea5e230-c0e1-4c04-af91-2a4fb07b8afc"), "neue deutsche welle" },
                { new("391a797d-50d7-46df-b380-39ccacd52a09"), "new romantic" },
                { new("56407f9d-3398-4bf3-bbbd-ea372fa5adeb"), "new wave" },
                { new("3188a961-88e0-4b12-9b9e-47f79e53d780"), "no wave" },
                { new("8e5474b7-f445-49a7-866d-0f8e34b0a519"), "noise rock" },
                { new("0cd76818-fdde-4f56-93a1-040f09ca7f23"), "noisecore" },
                { new("bbad9cda-bde2-448e-821c-7ebf85a54020"), "nu metal" },
                { new("290afc44-ad41-4d51-9ca6-4ee88df0f4f8"), "occult rock" },
                { new("221ff39c-4521-49e3-ab52-e4dde85efd1e"), "oi" },
                { new("2692f2d7-d680-4a5c-83f0-48e6e7c3ec6b"), "old school death metal" },
                { new("77365c92-f25b-4ed3-8d02-03aae8b6df0e"), "pop metal" },
                { new("60e21d01-ce21-4127-86b7-aada79558dab"), "pop punk" },
                { new("797e2e85-5ffd-495c-a757-8b4079052f0e"), "pop rock" },
                { new("bbb1310c-24cb-4fc9-97c4-f93e4549f1e2"), "post-grunge" },
                { new("aec382b7-b635-470c-9051-2f84e0b5ea1e"), "post-hardcore" },
                { new("e214700e-dae1-4b69-81e9-276e4e88fa32"), "post-metal" },
                { new("f6d8cadd-6a5e-4ec9-9c2b-996b5332a639"), "post-punk" },
                { new("0bfffa19-4e4a-4bf7-bf53-6ec44f0a2f8e"), "post-rock" },
                { new("f0b0aa5f-ed81-456b-85ab-492002a29eeb"), "power metal" },
                { new("4e6e388c-a9d2-45cb-80ad-bf64db4a0759"), "powerviolence" },
                { new("5c7785e6-a0b4-4441-a1e6-4fd4c0066a5f"), "progressive metal" },
                { new("ae9b8279-3959-48d8-8a88-741a7f6d4a48"), "progressive rock" },
                { new("146ef761-5ad9-48b4-b0b3-483104f7da48"), "psychedelic rock" },
                { new("8a14f570-7297-438d-996d-8797f9b8cfcc"), "psychobilly" },
                { new("8cc9b280-230b-4a3a-b1e2-8acab0744dd3"), "punk" },
                { new("bd7e1d40-43b1-4ba5-97b3-05b891466962"), "punk rock" },
                { new("2481fbdc-05ed-458d-9928-46f3336c343e"), "queercore" },
                { new("22623d74-4676-4853-aed9-921ae6fad495"), "rap metal" },
                { new("b4b184b7-6224-478e-a273-f9da9ee30136"), "rap rock" },
                { new("2cd7891d-c012-45f1-b1a1-86e34793f7fa"), "rapcore" },
                { new("0e3fc579-2d24-4f20-9dae-736e1ec78798"), "rock" },
                { new("e4581b76-a842-440b-9b0f-1aa7a00a5c57"), "rock and roll" },
                { new("f1a053eb-61bb-448a-964d-35784c4ae8c1"), "rockabilly" },
                { new("8fea0cb0-4205-49cb-95e8-bc1122857c7d"), "screamo" },
                { new("a4012b2e-5c27-4f81-8781-9ae5998d882c"), "shoegaze" },
                { new("455f264b-db00-4716-991d-fbd32dc24523"), "singer-songwriter" },
                { new("8d3d9402-79df-4d91-946c-ea041acfb3cc"), "slowcore" },
                { new("66544dbb-08a8-44c2-a21a-4d251481d2b6"), "sludge metal" },
                { new("7d0c6a6b-5d56-4b1e-a8c7-e32194f246b9"), "soft rock" },
                { new("905c1899-3027-4d29-8d9c-bbeef2c8628a"), "southern rock" },
                { new("532dcf83-5641-446d-b007-51292b401650"), "space rock" },
                { new("13b9a3d0-cadd-483d-9bf5-9c2b32a946a7"), "speed metal" },
                { new("5c76eb54-7342-4d1f-9d98-a93f2f6aa7f6"), "stoner metal" },
                { new("aa5f0254-403e-496a-a388-c01abf62f39a"), "stoner rock" },
                { new("c2ca3275-7e83-40cc-8f13-7ccae588144d"), "street punk" },
                { new("b76ee78d-ec97-49e9-8ea6-cfc46ecc3b23"), "surf rock" },
                { new("82e38e98-8a93-4ea9-a07f-ff0aeba07444"), "swamp rock" },
                { new("86a383d3-a4cc-4690-9a0f-f8ee83f7492d"), "symphonic black metal" },
                { new("2a4cae89-5a5a-44b7-9cf8-bf92c38a31dc"), "symphonic metal" },
                { new("166be36f-febb-4523-a005-1fb3603bd3f6"), "symphonic prog" },
                { new("f729e6f8-30dc-4b81-9ff4-f4e7de82225d"), "symphonic rock" },
                { new("db37c56d-e6a7-4809-b549-6f1cb559b559"), "technical death metal" },
                { new("cc4cf136-5690-4ee3-a62b-b172febfe322"), "thrash metal" },
                { new("e8a3ec42-4427-4365-8180-f1e05b46459e"), "thrashcore" },
                { new("fc736acd-9413-4b86-b8d4-47e9ce61d561"), "viking metal" },
                { new("823e4a7f-ade7-4d34-927a-721c60469b81"), "visual kei" },
                { new("6d39ad19-fbd6-407b-baf3-ad6ad46a5518"), "yacht rock" },
                { new("281e2c3f-80ad-4bc4-8b30-54222dee3b9c"), "zamrock" },
                { new("6992b6b1-c275-4728-89b0-8aebab5b7d94"), "zeuhl" },
            },

            [new("hh", "Hip hop")] = new()
            {
                { new("924943cd-73c8-45c0-96eb-74f2a15e5d6e"), "alternative hip hop" },
                { new("79fb8e4d-f508-40c3-9043-524811ee9897"), "bongo flava" },
                { new("d8048b43-9edb-4e1a-aa88-bb91fa847d4a"), "boom bap" },
                { new("ccd103a5-bf71-43a2-b715-244a81ac9950"), "bounce" },
                { new("8a2e8d9c-3402-4e83-b35d-a23e8016461a"), "chopped and screwed" },
                { new("c0e73bb6-37a4-485e-aaea-11c63b40c9f3"), "conscious hip hop" },
                { new("1e5e18e5-4e70-4a73-942a-c6c79c1aefe2"), "drill" },
                { new("6732427a-0d04-4a15-b9e5-b4b5b9532f8f"), "drill and bass" },
                { new("1bafab01-c092-47bc-84ee-a6b503b3cb86"), "east coast hip hop" },
                { new("f52f5fc5-3b75-4757-9d1f-4cabe6808856"), "emo rap" },
                { new("dfdf3e46-3b2b-417e-9346-86f9a5bac360"), "funk carioca" },
                { new("12b013ff-cf03-4fa0-8e1b-c9b7f5d27f90"), "hardcore hip hop" },
                { new("52faa157-6bad-4d86-a0ab-d4dec7d2513c"), "hip hop" },
                { new("66d01f06-21dc-4216-ac63-c5efb7b6338e"), "horrorcore" },
                { new("5f216b90-4ed2-4297-a243-d623db4c8235"), "industrial hip hop" },
                { new("6061fb1b-d733-4643-a3d4-940c9a5e2ab8"), "jazz rap" },
                { new("d92aaf30-5941-4a65-b741-d9d48c80772a"), "lo-fi hip hop" },
                { new("432290bd-f667-4871-9fdc-127996f3ad93"), "miami bass" },
                { new("9c8ba153-740e-4b88-b7ff-31d004944c95"), "nerdcore" },
                { new("38dd5ce2-96d8-4202-b477-75a857d78adb"), "phonk" },
                { new("da2fc3b4-b97d-491a-ac0f-8a731bee7237"), "pop rap" },
                { new("4080e55b-ac2e-4907-a724-491b3bfdba63"), "ragga hip-hop" },
                { new("9a339aa8-a0e5-450c-95a7-b11517bb508f"), "trap" },
                { new("cfe47781-460e-498a-92f5-399d68076727"), "turntablism" },
                { new("ca5d2892-93bf-4aae-85d5-a4589a1680b9"), "uk drill" },
                { new("03a8749c-93e6-4b71-8204-b33de116d8fd"), "underground hip hop" },
                { new("149a3524-0fd3-4bdc-9876-0361dfee8908"), "west coast hip hop" },
            },

            [new("rb", "R&B / Soul")] = new()
            {
                { new("650bda08-20ca-4012-b873-3b68c5768bdf"), "blue-eyed soul" },
                { new("fe12b346-a10e-450f-bf81-fa20894b5ea2"), "blues" },
                { new("310c2c5c-38a7-49ce-9d24-6f27e22bd203"), "boogie-woogie" },
                { new("15f9b448-6f2d-45a4-b11b-1fcdf45be203"), "classic blues" },
                { new("4bb4043a-0ee5-4b84-93fa-6ba4567a6ba0"), "contemporary r&b" },
                { new("a04d680b-b5c5-4f6a-87a3-f9f1ff98e3ea"), "disco" },
                { new("1696f12e-9f01-4309-be9d-116e31ac65b7"), "doo-wop" },
                { new("fe4ba6a1-9873-4fd0-a12b-a70c81818514"), "funk" },
                { new("1fbd6e82-0d33-4d78-a9b1-7d145a8eefa6"), "funk soul" },
                { new("b37aef4d-0029-4fc7-b634-6338197335a8"), "g-funk" },
                { new("bc8c2f79-dcea-43f3-962b-f0663868d42c"), "gangsta rap" },
                { new("0c44a04f-f7ba-46a0-9dbe-1a21359cfa1e"), "motown" },
                { new("286f9168-9c46-4e41-94a1-1aa4aeb93b8c"), "neo soul" },
                { new("dd497eac-e915-49a9-b1e4-482792acf397"), "new jack swing" },
                { new("a18a254b-67ca-4c8b-a13c-87154055e47e"), "northern soul" },
                { new("fadab481-3ced-466e-b828-74aed72d5c2f"), "p-funk" },
                { new("64e0bd76-822f-4549-a6fe-9858561282d1"), "pop soul" },
                { new("31be54b2-4d0c-42df-aa44-c496c7b4c3c3"), "r&b" },
                { new("d4eafdc7-1a86-422b-b598-79e6bb81745c"), "smooth soul" },
                { new("1b5e7b16-336b-473e-a364-5120413a9827"), "soul" },
                { new("bd61d502-8906-4073-a4ea-18c04e946dbc"), "southern soul" },
            },

            [new("pop", "Pop")] = new()
            {
                { new("d3ebe947-7c03-43f5-b333-09379881b0f6"), "alternative pop" },
                { new("930ef127-3653-4677-9b95-ecc90c7c1a14"), "art pop" },
                { new("0e7a3c6a-4fbc-4b66-8201-9196032768ac"), "avant-garde pop" },
                { new("f1c9b874-7ef2-4e79-9419-c0bcb5f33a30"), "britpop" },
                { new("6fd83b83-9b63-47fa-a208-5f9e762f40ab"), "bubblegum pop" },
                { new("78caea3b-84fd-4664-878a-fe70a6e1d9d1"), "cantopop" },
                { new("7232e5df-8466-421f-ad5a-ce2048e5522d"), "chillout" },
                { new("50e3ff0e-1abb-4577-9bf3-95d5c539ce2d"), "europop" },
                { new("abb2374f-30e5-4d3e-b417-2caf78ef1f6c"), "folk pop" },
                { new("05039e37-4520-4d1e-8173-92a7915a452b"), "hauntology" },
                { new("f390be72-360b-41bb-a310-6a2e638779d2"), "indie pop" },
                { new("eba7715e-ee26-4989-8d49-9db382955419"), "j-pop" },
                { new("b74b3b6c-0700-46b1-aa55-1f2869a3bd1a"), "k-pop" },
                { new("7054e8e2-7c7b-4432-83b9-93423b361792"), "kayōkyoku" },
                { new("2b9b7ea8-cb90-428c-adb2-d55b2faa4f1d"), "latin pop" },
                { new("924bddc4-5e86-46b8-adf7-ba23ed764c40"), "luk krung" },
                { new("489cf2f9-0edd-4fbc-aa05-ae248706cb3e"), "mandopop" },
                { new("6c76d9ec-0924-4fc9-bb22-91a695ca09d6"), "new age" },
                { new("1208a8e7-e5bd-4891-9f81-9a9fdfceeb39"), "noise pop" },
                { new("911c7bbb-172d-4df8-9478-dbff4296e791"), "pop" },
                { new("618704fc-daac-44a2-92c9-89e3b1a2fc8c"), "power pop" },
                { new("d698ece0-6fed-4262-b446-42c98b73b65a"), "psychedelic" },
                { new("2aeb5340-c474-4677-b9a6-35ddac9b6a58"), "psychedelic pop" },
                { new("7edde935-8094-409e-ba33-34779b4668f8"), "schlager" },
                { new("9864d280-a987-466f-bac2-ce103721c3e2"), "shibuya-kei" },
                { new("18e297f0-8fd3-46ac-9953-1a9a91b69596"), "space age pop" },
                { new("42fabe6b-0d6a-469d-b83c-21515bb2b490"), "swamp pop" },
                { new("4b7c8e31-462b-4e06-acfd-fa508ad26e0c"), "teen pop" },
                { new("e6d6a58f-6c00-484e-9bc7-12e2c4d024e2"), "yé-yé" },
            },

            [new("el", "Electronic / Dance")] = new()
            {
                { new("54c01942-22fd-4184-9877-1db0089b18f1"), "acid house" },
                { new("ba64013e-27bb-4f14-a530-8d25b296e0da"), "acid techno" },
                { new("8301f73c-9166-4108-bfeb-4fd22dc19083"), "alternative dance" },
                { new("608b0471-7531-4854-a348-e698c69cb699"), "ambient" },
                { new("9c143ab1-1506-4134-ba40-1660fbad7f51"), "ambient dub" },
                { new("cc2327cf-88a3-476b-950c-296c382eabd9"), "ambient house" },
                { new("1c6e4eae-915d-4662-a7ff-038920b67d9e"), "ambient techno" },
                { new("2d5cd6af-6195-4d12-9788-9a9b03ac0a12"), "bass house" },
                { new("decc967b-5b70-453b-9107-0b4e7a00505f"), "beat music" },
                { new("aac07ae0-8acf-4249-b5c0-2762b53947a2"), "big beat" },
                { new("43ef7823-5f16-4c9b-840d-f6b0de83a9c4"), "bleep techno" },
                { new("3b8f3f65-6caa-47c8-b076-fa59dfb69c43"), "breakbeat" },
                { new("1c5db6d3-9ce6-48c0-9bff-e29f7d7cb1af"), "breakbeat hardcore" },
                { new("8aad8a03-be38-48e4-aa43-5ca0f826c569"), "breakcore" },
                { new("63eddc75-dbad-4a05-a365-e30d1e87bca8"), "breaks" },
                { new("cf9daa9d-74e2-4eff-b0eb-7269bbca6501"), "broken beat" },
                { new("1dfc0cd3-bf39-4240-9265-96ced154425a"), "brostep" },
                { new("103a65bb-35d9-4d27-9c9b-f35a458621ba"), "chiptune" },
                { new("bab69b07-8bb9-4415-b666-c37609cef80b"), "club" },
                { new("d20c40f8-935f-43f3-80cb-b65857abb7fc"), "complextro" },
                { new("9d966bd4-89d5-4d6a-a408-eeb8c300c426"), "coupé-décalé" },
                { new("75858cef-eb30-4787-b524-3e04a41698cc"), "cyberpunk" },
                { new("e5bba957-8c91-496a-a675-c6d0c6b51c33"), "dance" },
                { new("b739a895-85ed-4ad3-8717-4e9ef5387dd8"), "dance-pop" },
                { new("01ecf8e4-7f7a-4145-ae97-504d3109633d"), "dark ambient" },
                { new("24db3574-7c54-4d9b-b238-8d17eb7e8d63"), "dark electro" },
                { new("7fa479e7-3d35-40d1-96f7-b619a5ca9d9f"), "deep house" },
                { new("88b01b1f-9151-4a1b-a9f7-608accdeaf20"), "detroit techno" },
                { new("6d12e828-40a5-4ebc-a5b0-aa5506e2efd0"), "digital hardcore" },
                { new("cc38aba3-48ed-439a-83b9-f81a34a66598"), "downtempo" },
                { new("462f9321-6103-49c9-b6db-96219bce6f62"), "drum and bass" },
                { new("c72a5d45-75a8-4b35-9f48-67e49eb4b5e5"), "dub" },
                { new("16aab171-3dce-454e-bb96-2636f15813fc"), "dub techno" },
                { new("1b50083b-1afa-4778-82c8-548b309af783"), "dubstep" },
                { new("c89a76aa-2149-499e-946b-8299aa2f4ab3"), "dungeon synth" },
                { new("f3818817-6235-48d5-82f7-b223eb0261d9"), "ebm" },
                { new("3976e9ba-16d0-461d-b732-99408a0374ba"), "edm" },
                { new("60f00d05-df4d-496e-8f5a-c45c03a56ad4"), "electro" },
                { new("05247982-e44f-4c58-96f8-a5453be69f5f"), "electro house" },
                { new("9b1af4d0-2196-43d4-bbca-845a2e02d962"), "electro swing" },
                { new("4f476710-cd29-400f-98fe-7782e1f00e06"), "electro-funk" },
                { new("6e2e809f-8c54-4e0f-aca0-0642771ab3cf"), "electro-industrial" },
                { new("cae57e98-c939-4b0e-ae49-8e0cb1bdc91d"), "electroclash" },
                { new("89255676-1f14-4dd8-bbad-fca839d6aff4"), "electronic" },
                { new("53a3cea3-17af-4421-a07a-5824b540aeb5"), "electronica" },
                { new("f9720c77-3574-4fe8-823b-c290b117222e"), "electronicore" },
                { new("3f3388b4-e36a-4bf8-a0be-66b10d92aaa4"), "electropop" },
                { new("845d2209-0b82-4a87-9405-453fc2e3086c"), "ethereal" },
                { new("8f8cbcde-3948-4be5-8bf0-8073f13e4b60"), "euro house" },
                { new("8de96944-cabb-4864-893a-ed072d802046"), "eurobeat" },
                { new("e9a67960-9b49-4967-a1c9-c26360e9c612"), "eurodance" },
                { new("6d588e79-30b0-4f8c-9924-d4a951d60aef"), "extratone" },
                { new("60cbcb55-852b-4329-9392-30a697802b71"), "fidget house" },
                { new("ec5a14c7-7793-46dc-b858-470183eb63f7"), "folktronica" },
                { new("405b97cf-e1d9-48aa-97cd-53212730589c"), "footwork" },
                { new("5acda04e-995d-4f79-9a66-5fe6a977ce15"), "french house" },
                { new("2fe74485-a939-4aa5-958f-ce09ba16906c"), "funktronica" },
                { new("62e4c7cb-a2b3-462b-8d96-1b0d66ee6cf3"), "funky house" },
                { new("e254f865-58ea-4483-9b2f-1bc65febcd71"), "future bass" },
                { new("ad6d7f8d-2224-4708-8a00-0fdd838d4676"), "future garage" },
                { new("cdc54f47-79fc-42e2-80d8-8ef58bd21122"), "future jazz" },
                { new("6df6e38b-2a6f-4990-94dc-ea48eba4f8ad"), "futurepop" },
                { new("2f2118d7-315b-4241-bf40-d5d6ca11348d"), "gabber" },
                { new("60c9c6a6-1de7-4010-9f5b-7692d3231d22"), "garage" },
                { new("092f40b5-9157-41f2-8243-68b704cd0ed1"), "garage house" },
                { new("75ddea23-a5f4-466c-951c-900d45fc352c"), "ghetto house" },
                { new("511a3aa8-abdb-4e63-b11c-2d50ab19b373"), "ghettotech" },
                { new("18b010d7-7d85-4445-a4a8-1889a4688308"), "glitch" },
                { new("aa3215ae-28e6-4993-908e-ed3807e1ce54"), "glitch hop" },
                { new("997baf8b-203b-4567-a475-1c1fe348d206"), "glitch pop" },
                { new("ade8f293-c049-438a-a5f0-b51e6776d9c5"), "goa trance" },
                { new("51cfaac4-6696-480b-8f1b-27cfc789109c"), "grime" },
                { new("bd477a3e-9235-4597-9ce5-3caa994baef7"), "happy hardcore" },
                { new("9c5052e0-36ff-4b92-9447-58f44ffdf682"), "hard house" },
                { new("c8acb78a-8337-4eae-9d19-48220dba03ac"), "hard trance" },
                { new("d3a28fe9-40f8-47d7-8fe9-b741307f5b49"), "hardbass" },
                { new("3308d0b0-3c7f-45e2-b835-5c2a0d433f02"), "hardcore techno" },
                { new("4a74a604-f702-424c-984f-93c5b04360ea"), "hardstyle" },
                { new("61d50e81-17bb-4b8b-ab36-7ae1759bc589"), "hi-nrg" },
                { new("b99d935b-cc28-404f-a306-50b7d468304f"), "hip house" },
                { new("a2782cb6-1cd0-477c-a61d-b3f8b42dd1b3"), "house" },
                { new("8eb583f1-4fd7-460c-8246-dcdccc0e3ef9"), "idm" },
                { new("da52c9d1-73f2-456e-a0d0-94bd854cb338"), "illbient" },
                { new("9e7ae1de-d5f2-46a5-b105-11c7367dc70d"), "indietronica" },
                { new("39b5ca66-873c-4a3d-94a3-3d31176b545e"), "industrial techno" },
                { new("97a4a75a-6e49-4519-9684-ae2ccb2fc0f9"), "italo-disco" },
                { new("36cdb9a1-a19e-4c8f-a887-c451e71cca46"), "juke" },
                { new("ea9b6ec3-fc8f-4c55-b2f9-19bd4511a986"), "jungle" },
                { new("32bea60c-e201-4241-8ed4-228636d34a97"), "leftfield" },
                { new("013161b9-3f21-468f-aa1f-28adf1eae085"), "madchester" },
                { new("13e409d0-fe4a-4dd4-a957-6e06462b085f"), "melodic trance" },
                { new("bbce54eb-a477-4b23-ac81-d3b9aac43756"), "microhouse" },
                { new("c1313278-b276-4a79-9fc1-770dd62a8b83"), "minimal techno" },
                { new("556be6ff-df60-4bb1-bd88-3ec7c69dfa28"), "minimal wave" },
                { new("edc96a6f-e8d0-4006-b065-d0fe188191c9"), "musique concrète" },
                { new("9094f251-8379-4ced-9541-a1c91045c327"), "neurofunk" },
                { new("4f2977aa-6943-404a-b8ec-295639ff0adf"), "nightcore" },
                { new("351424dd-91a3-4cf2-9edb-5df647743ec0"), "nu disco" },
                { new("848ecccf-e7fa-4439-a8ab-9dd3c31b4cf6"), "nu jazz" },
                { new("1e661112-612a-487a-a3de-180816200eab"), "onkyo" },
                { new("18527512-634e-48bb-81b2-6bfcf1818b95"), "power electronics" },
                { new("ceadd212-a669-4981-b05c-7e8889df55ae"), "power noise" },
                { new("70adb285-e1f7-458a-823f-5cbda5e291c4"), "progressive house" },
                { new("6bd228be-3374-4c4e-bc0a-46779fafe8f1"), "progressive trance" },
                { new("d1091878-2332-4f82-9a5c-f6d67cf4b7ef"), "psy-trance" },
                { new("b3cccf71-e843-4546-87fa-728055ba3147"), "psytrance" },
                { new("7aae791f-dfff-4910-b133-ba1b9e9c1560"), "ragga jungle" },
                { new("660e755a-1b66-440b-9b7c-fbcf245f4415"), "rave" },
                { new("9d4f50e8-5056-435e-8786-2d09e3407622"), "rhythmic noise" },
                { new("1deaf1c4-d01b-4a3c-adc4-ff45cf09264c"), "ritual ambient" },
                { new("23b9af26-1442-4dd9-8b40-a0db8a22e149"), "schranz" },
                { new("59c7776f-fd87-4cfb-a7a5-9fe66ec02b1b"), "speed garage" },
                { new("03544ca7-48ef-449b-abed-67aebdcd8120"), "speedcore" },
                { new("988e91a3-3341-416d-b7f8-7dbef6848dac"), "synth-pop" },
                { new("19565d07-8460-4773-8d54-149208a61bb4"), "synthwave" },
                { new("1f9c4b4a-c43d-40bd-b806-357153641fd0"), "tech house" },
                { new("ec039eb4-e175-44af-82ed-f152349eca35"), "tech trance" },
                { new("41fe3260-fcc1-450b-bd5a-803886c56912"), "techno" },
                { new("8a757534-0928-434c-accd-6be5517c1af3"), "terrorcore" },
                { new("2afc2320-384f-43a4-a71e-1b763c485045"), "trance" },
                { new("a21ea484-efba-4119-8120-429b26f98728"), "trap edm" },
                { new("94bd08a1-367d-49ad-a9f5-6db1c08f196a"), "tribal ambient" },
                { new("57159d7c-faa8-4266-abe1-7504e312fc88"), "tribal house" },
                { new("45eb1d9c-588c-4dc8-9394-a14b7c8f02bc"), "trip hop" },
                { new("31ec78f2-15c9-464b-a9e4-19e531028544"), "uk funky" },
                { new("95c57cd7-7fb8-4405-ade1-775c2d27e54e"), "uk garage" },
                { new("e837fa08-f314-428f-af96-ceed9858e3b2"), "vaporwave" },
                { new("9405d427-8b33-4670-a41a-040e23cef8ce"), "vocal house" },
                { new("f4dec9c1-abe2-4f3f-adee-5aa8f395d5ce"), "vocal trance" },
                { new("34ec5538-a91a-454f-b500-18253bb45c20"), "witch house" },
                { new("449e7cdf-0d7e-46aa-852d-96d81078abf3"), "wonky" },
            },

            [new("latin", "Latin")] = new()
            {
                { new("49fa872d-561c-4f8a-aea4-fbe7f67e1bd6"), "bolero" },
                { new("ae4e05d3-6ae6-41b2-9b63-7c9e27784e27"), "bolero son" },
                { new("e8a66eb6-882a-49fe-81d3-73f05a0a027f"), "chachachá" },
                { new("55d05e59-4829-4e98-8cb7-5d769cafb576"), "champeta" },
                { new("42419c8a-48ab-4b1e-a3d2-48b2a4d9ebdb"), "changüí" },
                { new("cbb6cb49-6671-46ba-8d35-c5f703d881a3"), "compas" },
                { new("7cd4173c-fef8-4e99-a527-98a25b6487c0"), "cumbia" },
                { new("09754a03-bf20-40e9-bfbe-ba1172d4a966"), "cumbia villera" },
                { new("396d532a-19e2-47da-9f05-f98ff961407d"), "descarga" },
                { new("2fb0a213-28f9-4e67-bb02-3df6d396d2de"), "fado" },
                { new("961c6dad-8cf0-47ab-bc09-ccd011f9ef28"), "flamenco" },
                { new("2cd172f7-d191-4cfd-8c4f-0a648b155fdf"), "guaguancó" },
                { new("e17eaeab-2d3f-4dae-8344-1dd5d9cb3c00"), "guajira" },
                { new("3c9097f9-10d9-4086-98e3-e8c54ef470b3"), "guaracha" },
                { new("7369bb8b-3399-4180-8cb2-4e37e0e7b0e7"), "latin" },
                { new("f6fb8e67-cd02-4fb4-b256-2970000dcb46"), "mambo" },
                { new("3f5f955d-f505-484a-b397-d0bf0760d8f3"), "merengue" },
                { new("7588308f-e08b-444e-8d6b-55549c58cf3c"), "milonga" },
                { new("7b447f98-1e94-4e57-9bb8-373743fbcdd8"), "mpb" },
                { new("3e226375-1286-40e3-92b3-d257e3a1babb"), "norteño" },
                { new("ebf28819-ebd3-4c00-9c42-3b8dc00f897c"), "nueva canción" },
                { new("4bcd6118-46ff-4903-a466-e1485005a3ba"), "pagode" },
                { new("ca20f188-68d2-4af5-966e-8e2e0f848758"), "ranchera" },
                { new("81638671-c67c-47cf-b21d-e92261068cf6"), "rocksteady" },
                { new("190bba5b-909f-4501-9a00-f96969983459"), "rumba" },
                { new("717f442d-59d1-4349-a14e-c4874acd3509"), "salsa" },
                { new("2447a0ec-e65f-48c7-b402-05cfe5c62cb7"), "samba" },
                { new("65877068-2116-4af6-89a2-838179a7e103"), "ska" },
                { new("89e34d8e-6562-412f-8da3-d040e159e6cd"), "ska punk" },
                { new("7aabc697-7c79-4f12-bb6f-ee5d70298a3f"), "skacore" },
                { new("b251cd9a-990c-49f6-8da3-db3457aa2114"), "soca" },
                { new("db48f46a-6467-49b0-a6b2-beb059584d9d"), "son cubano" },
                { new("7f0d2b34-d0ea-4ab5-a169-934f131bc40c"), "son montuno" },
                { new("267b14aa-3e3c-4615-803c-0255a81c8203"), "tango" },
                { new("6b8e3c88-add3-4b27-b2ec-a2135edf6e95"), "timba" },
                { new("bff33e5d-d591-498f-927f-b26c5527d817"), "vallenato" },
                { new("34d6235e-91cd-452f-95fb-2f8ce6508471"), "zouk" },
            },

            [new("rg", "Reggae / Dancehall")] = new()
            {
                { new("2346fb3e-8546-4119-a929-601db8e0a733"), "cloud rap" },
                { new("6c3503e3-bae3-42de-9e4c-7861f2053fbe"), "dancehall" },
                { new("97b15117-c3e2-47ec-9d64-79f90b528a9c"), "lovers rock" },
                { new("ce41a887-cdb3-4c52-bbce-604287d74573"), "mento" },
                { new("0d15e596-3374-4590-bdb0-0a95d38ab4dc"), "ragga" },
                { new("02b2f720-d06e-42ce-85c2-1ecd4191ffcb"), "reggae" },
                { new("bb14fb18-af6b-4547-9130-47fc4b208f48"), "reggaeton" },
                { new("f2d4991d-97fa-4c1c-8e09-15ec7668aa43"), "roots reggae" },
            },

            [new("ww", "World / Traditional")] = new()
            {
                { new("fcc58a18-9326-4c92-8b29-c294d44379c3"), "afrobeat" },
                { new("b413acf5-112a-4d44-a61f-9abe28f0dfac"), "bachata" },
                { new("9e44c2c9-8c44-4aa6-9de3-0f3e75b49f1b"), "baroque" },
                { new("b44a8ffd-e717-4d73-99ea-9fe5d245339d"), "calypso" },
                { new("b72d3108-3360-447e-bb08-0895deff7ae5"), "candombe" },
                { new("5b8be5dd-44b1-4461-9266-cfed3e16f55c"), "carnatic classical" },
                { new("dc726e7d-7f0d-47e8-9172-c9311d7bd4a8"), "chutney" },
                { new("b73d4a65-091e-4f08-8422-93f1b90f95d7"), "enka" },
                { new("1b757a82-3b28-4522-81d9-82d3d10b00c0"), "exotica" },
                { new("07b217fc-7798-4e74-aad6-2c08b32c2810"), "gagaku" },
                { new("92ccf7a7-c886-4e06-9aff-e432593653df"), "hindustani classical" },
                { new("53c3e4ea-ce29-4dc1-9133-09b6be18a607"), "joik" },
                { new("5c8b654c-0f2b-40d5-990b-e070a527b1df"), "kizomba" },
                { new("06eeec6e-f389-4783-b601-909bb10bd04e"), "klezmer" },
                { new("2b756f35-79bc-4424-8061-026b431d036a"), "luk thung" },
                { new("4c1cfd87-002d-49de-8e88-0f3cbcc090fe"), "maloya" },
                { new("16929e76-e49b-4ca3-9d26-11f7dc7e665a"), "maskanda" },  // [sic]
                { new("b0ac2e92-b41d-46e9-8c88-cd254f3a86e0"), "min'yō" },
                { new("bbe70cad-e184-42e5-9b90-e14438ba4363"), "pachanga" },
                { new("df6d324a-ff39-4055-9289-b14da57ab4e4"), "polka" },
                { new("ab01fbff-c7da-462a-93a9-71ad62b64d80"), "qawwali" },
                { new("deec5f2f-ccfc-4e0c-b1ac-8e5a17cd5cb7"), "raï" },
                { new("15d8d1e8-302d-4919-93a7-740099aef3c0"), "ryūkōka" },
            },

            [new("cy", "Country")] = new()
            {
                { new("5f9cba3d-1a9f-46cd-8c49-7ed78d1f3354"), "alternative country" },
                { new("2c765e49-a12e-45eb-b628-f8b9cc0a0250"), "americana" },
                { new("a1a7b40d-f6e6-48c1-a5fc-b15af73d7381"), "bluegrass" },
                { new("9cc9ea3b-e189-4def-9e47-86e9dc643eee"), "classic country" },
                { new("2f7f569a-9348-4384-8508-2d0ac7d6a778"), "contra" },
                { new("5f665615-7fb3-49d8-b541-62a7b239edbe"), "country" },
                { new("2de61e9c-de1d-47c8-999d-5b44c0cfeca1"), "country pop" },
                { new("ef7f6b36-cf15-408d-9eb4-fd7a46d0602b"), "gothic country" },
                { new("0582ba9e-320b-431d-a925-e1b22cad22d8"), "modern country" },
                { new("b5220905-bd0a-41d5-b534-ed5401be80c2"), "neo-traditional country" },
                { new("ed67ff3f-8f00-4321-badd-e9f183f7584b"), "old-time" },
                { new("f30d5ce4-559c-45c2-897c-089599ac9a83"), "outlaw country" },
                { new("037150a6-d83b-4307-908a-84201867186d"), "traditional country" },
            },

            [new("cl", "Classical")] = new()
            {
                { new("19d31930-9e5f-450d-8fa1-120e622773a5"), "bardcore" },
                { new("b7864789-29e6-4965-84e4-463baaa869df"), "chanson" },
                { new("bf9955d6-c3eb-466d-95ca-30f3848fc9ea"), "classical" },
                { new("5e6d5394-6508-4ce2-87ee-90388b1a6b9c"), "classical crossover" },
                { new("a190bb32-e0a7-4c56-b142-5cf9cfcfa713"), "contemporary classical" },
                { new("59b8a806-8bdd-45b5-978a-b5e404c910aa"), "honky tonk" },
                { new("4fc769b7-acac-4061-9129-ee8c6875b4c3"), "medieval" },
                { new("e328c306-8e35-46d2-93c3-623412ec7c13"), "modern classical" },
                { new("f3fe6570-1e79-4ed5-8eb9-9ed64d631871"), "orchestral" },
                { new("842008b8-1c67-4c8c-b450-7f5adf20fb7f"), "post-classical" },
                { new("bdebe8fd-c5b6-4b89-bb2a-ece612f1caa7"), "romantic classical" },
                { new("a4704a31-7c69-44d9-a590-8fcc2b258f77"), "slow waltz" },
                { new("50294a4d-61f0-4dc8-a20f-4708d52fe78a"), "symphony" },
                { new("4bf6058e-0736-4836-8eae-6d63bf78b9fb"), "waltz" },
            },

            [new("jazz", "Jazz")] = new()
            {
                { new("7dc2b20f-3953-4874-b9bf-41b8ba06d20c"), "acid jazz" },
                { new("8d901306-d4a3-436c-a1c3-75550e5b6eaf"), "avant-garde jazz" },
                { new("ac30756b-55a7-4dc3-8908-8f94c93318d0"), "bebop" },
                { new("e7bb9a21-9556-4b85-bc78-5f69a3d0577d"), "big band" },
                { new("573a284f-2ae3-4636-af36-10fc1078d295"), "bossa nova" },
                { new("f0e90b41-4499-493f-b6e3-05d3c3ea1b20"), "classic jazz" },
                { new("e2b06d21-13df-4a48-aa81-6ddd99344dc7"), "contemporary jazz" },
                { new("5fc86669-d9ba-4716-8090-eff39081090e"), "cool jazz" },
                { new("2126f16b-13b6-45c9-b02e-c63be8bbdaea"), "dansband" },
                { new("fd7b5582-e69c-40ad-90b1-d31db33be612"), "dark jazz" },
                { new("3c31635b-a5a7-44eb-ab9d-4bbde11da2d4"), "free jazz" },
                { new("8c083468-2dce-4a31-ae77-433af3deafd3"), "hard bop" },
                { new("c1a4860e-720f-4f4c-8a54-a8032aac08fa"), "instrumental jazz" },
                { new("a715278f-1580-409f-8078-4ffbc800e08b"), "jazz" },
                { new("2377ff19-83e7-4873-b425-dce6d1ad4ef4"), "jazz fusion" },
                { new("f3c43ae8-839a-4523-b295-cfa4eba6c911"), "jazz-funk" },
                { new("48095aaf-c31d-4bfc-9578-8755c71d44af"), "latin jazz" },
                { new("94ce0ba0-9043-4b5b-87d6-3c1a8458f75d"), "post-bop" },
                { new("a1f1dcc8-6078-4c12-90b7-47e3080e330c"), "smooth jazz" },
                { new("e70f0aa3-9531-448c-8182-7ff34e3d74f8"), "soul jazz" },
                { new("063d0461-1e12-4246-87bf-69c5ca611cc2"), "swing" },
                { new("782dc157-d1a5-42b9-9174-d329c24a7ef3"), "vocal jazz" },
                { new("bb071d13-8c80-411b-9c0b-16b37bf71ae6"), "west coast swing" },
                { new("60e19df0-99e8-4863-be31-7ec7e5c98c7a"), "western swing" },
            },

            [new("blues", "Blues / Folk")] = new()
            {
                { new("37f85b9c-c3fc-4b5a-8545-51aeb78c8786"), "acoustic blues" },
                { new("d2d6de50-4dd8-4838-92ac-aa6d1e600bba"), "afoxê" },
                { new("0b48a36c-630f-4ee7-8cf3-480e3dd8be65"), "alternative folk" },
                { new("59a849e1-73fe-428d-8a61-3788875f9644"), "bhangra" },
                { new("0af301ff-3e33-4f5f-934f-5916cfc04157"), "cajun" },
                { new("34edc6ac-d431-4b36-be15-ecd17533e5e2"), "celtic" },
                { new("f1e26a78-3e85-4365-a34d-dc4b5389b29d"), "chicago blues" },
                { new("c11e4253-a888-4a2a-adde-c274b748d0b3"), "contemporary folk" },
                { new("1a46845c-ed4d-48c5-a93f-45bb85cf56d1"), "country blues" },
                { new("984e6cb2-823e-4709-a23b-233f7f2ee3aa"), "country folk" },
                { new("16103855-c0f1-4221-81d8-eafc53b1b41c"), "dark folk" },
                { new("b0702eb4-2951-4e1f-bbcd-dd9b0ec63c7c"), "delta blues" },
                { new("8164d906-055d-4ca8-9ab2-14204b301cc3"), "electric blues" },
                { new("18a0d235-c8e0-4575-b0dc-e3bb0cc2ad2c"), "filk" },
                { new("a91eed3a-fdd0-494e-ade0-95ea4117f6eb"), "folk" },
                { new("8103927c-5f4b-4bbc-9d8c-7a1241d2bab8"), "freak folk" },
                { new("36917620-240a-4fed-abcb-7be611ac3834"), "indie folk" },
                { new("3d88ed41-624d-4be9-bf9d-ca11991eb250"), "irish folk" },
                { new("9702d99a-823f-47ba-b8c2-af0538c9640b"), "jazz blues" },
                { new("6fb5fe60-b6c2-414f-8011-4f8dc023039b"), "modern blues" },
                { new("e94c28db-21a2-43de-9e58-8113fba2c732"), "neofolk" },
                { new("34d08b64-7426-42e1-959e-48af44352fcd"), "progressive folk" },
                { new("507757f0-a63c-422b-93f1-f1d77462033f"), "psychedelic folk" },
                { new("d017dd87-704b-4ad4-8b67-39fb401b8339"), "ragtime" },
                { new("498229df-4956-4ed7-91f7-a9d4b1877af1"), "zydeco" },
            },

            [new("cm", "Comedy / Spoken word")] = new()
            {
                { new("966d3d2d-15bd-4f6f-a830-15ab2c5e662f"), "audiobook" },
                { new("17b58d38-1110-4f5a-9412-b0314329622b"), "ballad" },
                { new("119ecf69-7d87-4f63-bc83-2959a0111a55"), "comedy" },
                { new("c774791e-1b78-4794-a109-befb712ef44d"), "spoken word" },
            },

            [new("ch", "Christian / Gospel")] = new()
            {
                { new("c02cd2c5-9316-4046-a241-f74b773fee06"), "christian rock" },
                { new("2b4f0036-79a0-4ce0-a38c-4c0c8e61c579"), "contemporary christian" },
                { new("6aeacd9a-3964-4ca4-9e3c-23ec8bd8848d"), "contemporary gospel" },
                { new("08289b42-7e41-4987-9263-c28bb4c59da0"), "gospel" },
            },

            [new("st", "Soundtracks")] = new()
            {
                { new("ff6d73e8-bf1a-431e-9911-88ae7ffcfdfb"), "musical" },
                { new("56e0b906-1018-4a55-8ec7-86746272116e"), "opera" },
                { new("5a56d32f-81e4-4301-be70-4a060ab3b4a0"), "production music" },
                { new("db8a5e56-60da-4438-8e2a-f2f51f0d4092"), "vaudeville" },
            },

            [new("kids", "Kids'")] = new()
            {

            },

            [new("more", "More")] = new()
            {
                { new("b1c0c2ad-e5ae-4bb6-a1be-d66e195fd2d7"), "avant-garde" },
                { new("f8f6157d-5ace-4844-90c2-2e2a829b8932"), "barbershop" },
                { new("31aee5bc-c1e8-43d7-a7bc-902df802ccbb"), "christmas music" },
                { new("75c5866f-0212-4c02-9650-de475dda157e"), "drone" },
                { new("9e5374b1-1f09-4c27-827a-d4e6aff4cc29"), "drone metal" },
                { new("68c81274-5770-4e7b-a4bf-ab0d7d425d99"), "experimental" },
                { new("6a1ba2f9-1e5b-446f-9ffa-c0b0963aa08f"), "free improvisation" },
                { new("7af339eb-e666-4aa6-a6ca-8444ca573fe0"), "fusion" },
                { new("5c61cf56-7dcd-4bd8-87e4-b5bb0c25255a"), "harsh noise" },
                { new("344dc18a-b982-49e7-918c-23c7d69fb6ba"), "harsh noise wall" },
                { new("538c905b-642d-43c9-b071-439af11cc471"), "indie" },
                { new("060beed7-e597-4c42-8e25-5bf8bd5dd3cb"), "industrial" },
                { new("b97dda04-d712-4527-80fe-7cfa0e71ecf1"), "industrial musical" },
                { new("4ae63770-68af-4fae-b0f1-5e53f372c743"), "instrumental" },
                { new("e1e37f00-9bd9-4510-b6ca-9bb128b665f4"), "line dance" },
                { new("65e2f3ad-e0bd-4627-bd83-cba7c8b6d011"), "lo-fi" },
                { new("de2785b2-f282-4718-a422-37dc93964bdd"), "lounge" },
                { new("449e6224-5a56-4146-9463-3feda95a5f15"), "lowercase" },
                { new("482ff4ed-fc37-49fb-9858-92f955a19e6c"), "martial industrial" },
                { new("299c1b38-5c7b-4dfc-ab3a-f541c936987a"), "microsound" },
                { new("8d9a00db-0b8b-41ff-9af7-d0f96793bdc5"), "minimal" },
                { new("89c7c9b3-215e-482f-bce3-3219ffd0a222"), "noise" },
                { new("018b7630-8941-4966-90e4-3774daea4e04"), "non-music" },
                { new("8302fd70-150f-4f74-b11c-10a7ff7586b9"), "plunderphonics" },
                { new("aef2077e-6cbc-47ba-9698-9eb5f842a412"), "pornogrind" },
                { new("8d275a8d-154a-4bcb-8b5a-f56306ad4bcb"), "progressive" },
                { new("518102e9-8d09-4f6b-b1e2-141bdcce6a82"), "red song" },
                { new("3571d5f1-b4f1-446c-87f3-5033c3ea0bcd"), "spectralism" },
                { new("8a729b73-1ef1-44da-a0d8-ce90cfc181c3"), "steampunk" },
            }
        };
    }
}
