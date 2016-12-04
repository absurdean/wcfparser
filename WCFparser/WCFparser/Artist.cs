using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace WCFparser
{
    [DataContract]
    public class Artist
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<string> TopTracks { get; set; }
        [DataMember]
        public List<string> Genres { get; set; }
        [DataMember]
        public List<string> TopAlbums { get; set; }
        [DataMember]
        public List<string> SimilarArtists { get; set; }
        [DataMember]
        public string Bio { get; set; }
    }
}