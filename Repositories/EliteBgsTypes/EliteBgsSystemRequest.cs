using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Repositories.EliteBgsTypes.SystemRequest
{
    public partial class EliteBgsSystemRequest
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

        [JsonProperty("x")]
        public double X { get; set; }

        [JsonProperty("y")]
        public double Y { get; set; }

        [JsonProperty("z")]
        public double Z { get; set; }

        [JsonProperty("__v")]
        public long V { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("government")]
        public string Government { get; set; }

        [JsonProperty("allegiance")]
        public string Allegiance { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("security")]
        public string Security { get; set; }

        [JsonProperty("population")]
        public long Population { get; set; }

        [JsonProperty("primary_economy")]
        public string PrimaryEconomy { get; set; }

        [JsonProperty("controlling_minor_faction")]
        public string ControllingMinorFaction { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("eddb_id")]
        public long EddbId { get; set; }

        [JsonProperty("factions")]
        public List<Faction> Factions { get; set; }

        [JsonProperty("secondary_economy")]
        public string SecondaryEconomy { get; set; }

        [JsonProperty("system_address")]
        public string SystemAddress { get; set; }

        [JsonProperty("conflicts")]
        public Conflict[] Conflicts { get; set; }
    }

    public partial class Faction
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("name_lower")]
        public string NameLower { get; set; }
    }

    public partial class Conflict
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("faction1")]
        public Faction1Class Faction1 { get; set; }

        [JsonProperty("faction2")]
        public Faction1Class Faction2 { get; set; }
    }

    public partial class Faction1Class
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("name_lower")]
        public string NameLower { get; set; }

        [JsonProperty("stake")]
        public string Stake { get; set; }

        [JsonProperty("stake_lower")]
        public string StakeLower { get; set; }

        [JsonProperty("days_won")]
        public long DaysWon { get; set; }
    }

    public partial class FactionElement
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("name_lower")]
        public string NameLower { get; set; }
    }

    public partial class EliteBgsSystemRequest
    {
        public static EliteBgsSystemRequest FromJson(string json) => JsonConvert.DeserializeObject<EliteBgsSystemRequest>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this EliteBgsSystemRequest self) => JsonConvert.SerializeObject(self, Converter.Settings);
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
