namespace Repositories.EliteBgsTypes.TickRequest
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class EliteBgsTickRequest
    {
        [JsonProperty("_id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty("time", Required = Required.Always)]
        public DateTimeOffset Time { get; set; }

        [JsonProperty("updated_at", Required = Required.Always)]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("__v", Required = Required.Always)]
        public long V { get; set; }
    }

    public partial class EliteBgsTickRequest
    {
        public static List<EliteBgsTickRequest> FromJson(string json) => JsonConvert.DeserializeObject<List<EliteBgsTickRequest>>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<EliteBgsTickRequest> self) => JsonConvert.SerializeObject(self, Converter.Settings);
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