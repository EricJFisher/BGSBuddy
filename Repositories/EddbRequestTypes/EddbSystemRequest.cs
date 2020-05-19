namespace Repositories.EddbRequestTypes
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class EddbSystemRequest
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

        [JsonProperty("id")]
        public long DocId { get; set; }

        [JsonProperty("name_lower")]
        public string NameLower { get; set; }

        [JsonProperty("minor_faction_presences")]
        public List<MinorFactionPresence> MinorFactionPresences { get; set; }

        [JsonProperty("reserve_type")]
        public string ReserveType { get; set; }

        [JsonProperty("reserve_type_id")]
        public long? ReserveTypeId { get; set; }

        [JsonProperty("controlling_minor_faction")]
        public string ControllingMinorFaction { get; set; }

        [JsonProperty("controlling_minor_faction_id")]
        public long ControllingMinorFactionId { get; set; }

        [JsonProperty("simbad_ref")]
        public SimbadRef SimbadRef { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("needs_permit")]
        public bool NeedsPermit { get; set; }

        [JsonProperty("power_state_id")]
        public long? PowerStateId { get; set; }

        [JsonProperty("power_state")]
        public string PowerState { get; set; }

        [JsonProperty("power")]
        public string Power { get; set; }

        [JsonProperty("primary_economy")]
        public string PrimaryEconomy { get; set; }

        [JsonProperty("primary_economy_id")]
        public long PrimaryEconomyId { get; set; }

        [JsonProperty("security")]
        public Security Security { get; set; }

        [JsonProperty("security_id")]
        public long SecurityId { get; set; }

        [JsonProperty("states")]
        public List<State> States { get; set; }

        [JsonProperty("allegiance")]
        public Allegiance Allegiance { get; set; }

        [JsonProperty("allegiance_id")]
        public long AllegianceId { get; set; }

        [JsonProperty("government")]
        public string Government { get; set; }

        [JsonProperty("government_id")]
        public long GovernmentId { get; set; }

        [JsonProperty("is_populated")]
        public bool IsPopulated { get; set; }

        [JsonProperty("population")]
        public long Population { get; set; }

        [JsonProperty("z")]
        public double Z { get; set; }

        [JsonProperty("y")]
        public double Y { get; set; }

        [JsonProperty("x")]
        public double X { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("edsm_id")]
        public long EdsmId { get; set; }

        [JsonProperty("__v")]
        public long V { get; set; }
    }

    public partial class MinorFactionPresence
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("happiness_id")]
        public long HappinessId { get; set; }

        [JsonProperty("minor_faction_id")]
        public long MinorFactionId { get; set; }

        [JsonProperty("influence")]
        public double Influence { get; set; }

        [JsonProperty("active_states")]
        public List<State> ActiveStates { get; set; }

        [JsonProperty("pending_states")]
        public List<State> PendingStates { get; set; }

        [JsonProperty("recovering_states")]
        public List<State> RecoveringStates { get; set; }
    }

    public partial class State
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("id")]
        public long StateId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("name_lower")]
        public string NameLower { get; set; }
    }

    public enum Allegiance { Alliance, Empire, Federation, Independent, PilotsFederation, Unknown };

    public enum Security { Anarchy, Low, Medium, High, Unknown };

    public enum SimbadRef { Cd457854, Empty, Hip61097, Ltt5058, Unknown };

    public partial class EddbSystemRequest
    {
        public static EddbSystemRequest FromJson(string json) => JsonConvert.DeserializeObject<EddbSystemRequest>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this EddbSystemRequest self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                AllegianceConverter.Singleton,
                SecurityConverter.Singleton,
                SimbadRefConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class AllegianceConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Allegiance) || t == typeof(Allegiance?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "alliance":
                    return Allegiance.Alliance;
                case "empire":
                    return Allegiance.Empire;
                case "federation":
                    return Allegiance.Federation;
                case "independent":
                    return Allegiance.Independent;
                case "pilots federation":
                    return Allegiance.PilotsFederation;
                default:
                    return Allegiance.Unknown;
            }
            throw new Exception("Cannot unmarshal type Allegiance");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Allegiance)untypedValue;
            switch (value)
            {
                case Allegiance.Alliance:
                    serializer.Serialize(writer, "alliance");
                    return;
                case Allegiance.Independent:
                    serializer.Serialize(writer, "independent");
                    return;
            }
            throw new Exception("Cannot marshal type Allegiance");
        }

        public static readonly AllegianceConverter Singleton = new AllegianceConverter();
    }

    internal class SecurityConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Security) || t == typeof(Security?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "anarchy":
                    return Security.Anarchy;
                case "low":
                case "$system_security_low;":
                    return Security.Low;
                case "medium":
                    return Security.Medium;
                case "high":
                    return Security.High;
                default:
                    return Security.Unknown;
            }
            throw new Exception("Cannot unmarshal type Security");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Security)untypedValue;
            switch (value)
            {
                case Security.Low:
                    serializer.Serialize(writer, "low");
                    return;
                case Security.Medium:
                    serializer.Serialize(writer, "medium");
                    return;
            }
            throw new Exception("Cannot marshal type Security");
        }

        public static readonly SecurityConverter Singleton = new SecurityConverter();
    }

    internal class SimbadRefConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(SimbadRef) || t == typeof(SimbadRef?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "":
                    return SimbadRef.Empty;
                case "cd-45 7854":
                    return SimbadRef.Cd457854;
                case "hip 61097":
                    return SimbadRef.Hip61097;
                case "ltt 5058":
                    return SimbadRef.Ltt5058;
                default:
                    return SimbadRef.Unknown;
            }
            throw new Exception("Cannot unmarshal type SimbadRef");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (SimbadRef)untypedValue;
            switch (value)
            {
                case SimbadRef.Empty:
                    serializer.Serialize(writer, "");
                    return;
                case SimbadRef.Cd457854:
                    serializer.Serialize(writer, "cd-45 7854");
                    return;
                case SimbadRef.Hip61097:
                    serializer.Serialize(writer, "hip 61097");
                    return;
            }
            throw new Exception("Cannot marshal type SimbadRef");
        }

        public static readonly SimbadRefConverter Singleton = new SimbadRefConverter();
    }
}