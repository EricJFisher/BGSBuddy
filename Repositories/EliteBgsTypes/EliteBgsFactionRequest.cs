using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Repositories.EliteBgsTypes
{
    public partial class EliteBgsFactionRequest
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

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("__v")]
        public long V { get; set; }

        [JsonProperty("name_lower")]
        public string NameLower { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("government")]
        public string Government { get; set; }

        [JsonProperty("allegiance")]
        public string Allegiance { get; set; }

        [JsonProperty("eddb_id")]
        public long EddbId { get; set; }

        [JsonProperty("faction_presence")]
        public List<FactionPresence> FactionPresence { get; set; }
    }

    public partial class FactionPresence
    {
        [JsonProperty("system_name")]
        public string SystemName { get; set; }

        [JsonProperty("system_name_lower")]
        public string SystemNameLower { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("influence")]
        public double Influence { get; set; }

        [JsonProperty("happiness")]
        public Happiness Happiness { get; set; }

        [JsonProperty("active_states")]
        public List<ActiveState> ActiveStates { get; set; }

        [JsonProperty("pending_states")]
        public List<IngState> PendingStates { get; set; }

        [JsonProperty("recovering_states")]
        public List<IngState> RecoveringStates { get; set; }

        [JsonProperty("conflicts")]
        public List<Conflict> Conflicts { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
    }

    public partial class ActiveState
    {
        [JsonProperty("state")]
        public string State { get; set; }
    }

    public partial class Conflict
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("opponent_name")]
        public string OpponentName { get; set; }

        [JsonProperty("opponent_name_lower")]
        public string OpponentNameLower { get; set; }

        [JsonProperty("stake")]
        public string Stake { get; set; }

        [JsonProperty("stake_lower")]
        public string StakeLower { get; set; }

        [JsonProperty("days_won")]
        public long DaysWon { get; set; }
    }

    public partial class IngState
    {
        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("trend")]
        public long Trend { get; set; }
    }

    public enum Happiness { FactionHappinessband1, FactionHappinessband2, FactionHappinessband3 };

    public partial class EliteBgsFactionRequest
    {
        public static EliteBgsFactionRequest FromJson(string json) => JsonConvert.DeserializeObject<EliteBgsFactionRequest>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this EliteBgsFactionRequest self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                HappinessConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class HappinessConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Happiness) || t == typeof(Happiness?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "$faction_happinessband1;":
                    return Happiness.FactionHappinessband1;
                case "$faction_happinessband2;":
                    return Happiness.FactionHappinessband2;
                case "$faction_happinessband3;":
                    return Happiness.FactionHappinessband3;
            }
            throw new Exception("Cannot unmarshal type Happiness");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Happiness)untypedValue;
            switch (value)
            {
                case Happiness.FactionHappinessband2:
                    serializer.Serialize(writer, "$faction_happinessband2;");
                    return;
                case Happiness.FactionHappinessband3:
                    serializer.Serialize(writer, "$faction_happinessband3;");
                    return;
            }
            throw new Exception("Cannot marshal type Happiness");
        }

        public static readonly HappinessConverter Singleton = new HappinessConverter();
    }
}
