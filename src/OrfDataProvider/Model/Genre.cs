using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OrfDataProvider.Model
{
    public class Genre
    {
        public int Id { get; set; }

        [JsonPropertyName("advertising_mapping")]
        public AdvertisingMapping TheAdvertisingMapping { get; set; }
        public int Sorting { get; set; }
        public string Title { get; set; }
        public DateTime UpdatedAt { get; set; }
        [JsonPropertyName("_links")]
        public Links TheLinks { get; set; }

        public class AdvertisingMapping
        {
            public Live Live { get; set; }
            public Vod Vod { get; set; }
        }

        public class Live
        {
            public Platform Web { get; set; }
            public Platform Mob { get; set; }
            public Platform App { get; set; }
            public Smart Smart { get; set; }
        }

        public class Vod
        {
            public Platform Web { get; set; }
            public Platform Mob { get; set; }
            public Platform App { get; set; }
            public Smart Smart { get; set; }
        }

        public class Platform
        {
            public int Sb { get; set; }
            public int Tbar { get; set; }
            public int Pre { get; set; }
            public int Post { get; set; }
            public int Par { get; set; }
        }

        public class Smart
        {
            public int Pre { get; set; }
            public int Post { get; set; }
        }

        public class Links
        {
            public Href Self { get; set; }
            public Href Image { get; set; }
            public Href Profiles { get; set; }
            public Href Episodes { get; set; }
            public Href LatestEpisode { get; set; }
            public Href Tags { get; set; }
            public Href AdvertisingTags { get; set; }
        }

        public class Href
        {
            [JsonPropertyName("href")]
            public string TheHref { get; set; }
        }
    }
}
