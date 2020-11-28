namespace Repositories.EddnRequestTypes
{
    using System;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class EddnRequest
    {
        [JsonProperty("$schemaRef")]
        public Uri SchemaRef { get; set; }

        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }
    }

    public partial class Header
    {
        [JsonProperty("gatewayTimestamp")]
        public DateTimeOffset GatewayTimestamp { get; set; }

        [JsonProperty("softwareName")]
        public string SoftwareName { get; set; }

        [JsonProperty("softwareVersion")]
        public string SoftwareVersion { get; set; }

        [JsonProperty("uploaderID")]
        public string UploaderId { get; set; }
    }

    public partial class Message
    {
        [JsonProperty("Body")]
        public string Body { get; set; }

        [JsonProperty("BodyID")]
        public long BodyId { get; set; }

        [JsonProperty("BodyType")]
        public string BodyType { get; set; }

        [JsonProperty("Conflicts")]
        public Conflict[] Conflicts { get; set; }

        [JsonProperty("Factions")]
        public FactionElement[] Factions { get; set; }

        [JsonProperty("Population")]
        public long Population { get; set; }

        [JsonProperty("StarPos")]
        public double[] StarPos { get; set; }

        [JsonProperty("StarSystem")]
        public string StarSystem { get; set; }

        [JsonProperty("SystemAddress")]
        public long SystemAddress { get; set; }

        [JsonProperty("SystemAllegiance")]
        public string SystemAllegiance { get; set; }

        [JsonProperty("SystemEconomy")]
        public string SystemEconomy { get; set; }

        [JsonProperty("SystemFaction")]
        public SystemFaction SystemFaction { get; set; }

        [JsonProperty("SystemGovernment")]
        public string SystemGovernment { get; set; }

        [JsonProperty("SystemSecondEconomy")]
        public string SystemSecondEconomy { get; set; }

        [JsonProperty("SystemSecurity")]
        public string SystemSecurity { get; set; }

        [JsonProperty("event")]
        public string Event { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    public partial class Conflict
    {
        [JsonProperty("Faction1")]
        public Faction Faction1 { get; set; }

        [JsonProperty("Faction2")]
        public Faction Faction2 { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("WarType")]
        public string WarType { get; set; }
    }

    public partial class Faction
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Stake")]
        public string Stake { get; set; }

        [JsonProperty("WonDays")]
        public long WonDays { get; set; }
    }

    public partial class FactionElement
    {
        [JsonProperty("Allegiance")]
        public string Allegiance { get; set; }

        [JsonProperty("FactionState")]
        public string FactionState { get; set; }

        [JsonProperty("Government")]
        public string Government { get; set; }

        [JsonProperty("Happiness")]
        public string Happiness { get; set; }

        [JsonProperty("Influence")]
        public double Influence { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("ActiveStates", NullValueHandling = NullValueHandling.Ignore)]
        public ActiveState[] ActiveStates { get; set; }
    }

    public partial class ActiveState
    {
        [JsonProperty("State")]
        public string State { get; set; }
    }

    public partial class SystemFaction
    {
        [JsonProperty("FactionState")]
        public string FactionState { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }
    }

    public partial class EddnRequest
    {
        public static EddnRequest FromJson(string json) => JsonConvert.DeserializeObject<EddnRequest>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this EddnRequest self) => JsonConvert.SerializeObject(self, Converter.Settings);
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