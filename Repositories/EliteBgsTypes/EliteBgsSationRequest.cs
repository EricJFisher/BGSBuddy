using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Repositories.EliteBgsTypes.StationRequest
{
    public partial class EliteBgsStationRequest
    {
        [JsonProperty("docs")]
        public List<Doc> Docs { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("limit")]
        public long Limit { get; set; }

        [JsonProperty("page")]
        public long Page { get; set; }

        [JsonProperty("pages")]
        public long Pages { get; set; }
    }

    public partial class Doc
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("name_lower")]
        public string NameLower { get; set; }

        [JsonProperty("system_lower")]
        public string SystemLower { get; set; }

        [JsonProperty("__v")]
        public long V { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("system")]
        public string System { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("government")]
        public string Government { get; set; }

        [JsonProperty("economy")]
        public string Economy { get; set; }

        [JsonProperty("allegiance")]
        public string Allegiance { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("distance_from_star")]
        public double DistanceFromStar { get; set; }

        [JsonProperty("controlling_minor_faction")]
        public string ControllingMinorFaction { get; set; }

        [JsonProperty("services")]
        public List<Service> Services { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("eddb_id")]
        public long EddbId { get; set; }

        [JsonProperty("all_economies")]
        public List<AllEconomy> AllEconomies { get; set; }

        [JsonProperty("market_id")]
        public string MarketId { get; set; }
    }

    public partial class AllEconomy
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("proportion")]
        public long Proportion { get; set; }
    }

    public partial class Service
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("name_lower")]
        public string NameLower { get; set; }
    }

    public partial class EliteBgsStationRequest
    {
        public static EliteBgsStationRequest FromJson(string json) => JsonConvert.DeserializeObject<EliteBgsStationRequest>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this EliteBgsStationRequest self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
