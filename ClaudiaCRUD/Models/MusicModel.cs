using Newtonsoft.Json;
using System;

namespace ClaudiaCRUD.Models
{
    public class MusicModel
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public string Id;

        [JsonProperty(PropertyName = "singer")]
        public string Singer;

        [JsonProperty(PropertyName = "album")]
        public string Album;

        [JsonProperty(PropertyName = "song")]
        public string Song;

        [JsonProperty(PropertyName = "genre")]
        public string Genre;
        
        [JsonProperty(PropertyName = "release")]
        public string Release;

        [JsonProperty(PropertyName = "createDate")]
        public DateTime CreateDate;

    }
}
