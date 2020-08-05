using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ingeniOracleBackEnd.Models.Json {
    public partial class RespMenuJsonModel {
        [JsonProperty ("result")]
        public Result[] Result { get; set; }

        [JsonProperty ("id")]
        public long Id { get; set; }

        [JsonProperty ("exception")]
        public object Exception { get; set; }

        [JsonProperty ("status")]
        public long Status { get; set; }

        [JsonProperty ("isCanceled")]
        public bool IsCanceled { get; set; }

        [JsonProperty ("isCompleted")]
        public bool IsCompleted { get; set; }

        [JsonProperty ("isCompletedSuccessfully")]
        public bool IsCompletedSuccessfully { get; set; }

        [JsonProperty ("creationOptions")]
        public long CreationOptions { get; set; }

        [JsonProperty ("asyncState")]
        public object AsyncState { get; set; }

        [JsonProperty ("isFaulted")]
        public bool IsFaulted { get; set; }
    }

    public partial class Result {
        [JsonProperty ("id")]
        [JsonConverter (typeof (ParseStringConverter))]
        public long Id { get; set; }

        [JsonProperty ("appName")]
        public string AppName { get; set; }

        [JsonProperty ("img")]
        public string Img { get; set; }
    }

    public partial class RespMenuJsonModel {
        public static RespMenuJsonModel FromJson (string json) => JsonConvert.DeserializeObject<RespMenuJsonModel> (json, ingeniOracleBackEnd.Models.Json.Converter.Settings);
    }

    public static class Serialize {
        public static string ToJson (this RespMenuJsonModel self) => JsonConvert.SerializeObject (self, ingeniOracleBackEnd.Models.Json.Converter.Settings);
    }

    internal static class Converter {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter {
        public override bool CanConvert (Type t) => t == typeof (long) || t == typeof (long?);

        public override object ReadJson (JsonReader reader, Type t, object existingValue, JsonSerializer serializer) {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string> (reader);
            long l;
            if (Int64.TryParse (value, out l)) {
                return l;
            }
            throw new Exception ("Cannot unmarshal type long");
        }

        public override void WriteJson (JsonWriter writer, object untypedValue, JsonSerializer serializer) {
            if (untypedValue == null) {
                serializer.Serialize (writer, null);
                return;
            }
            var value = (long) untypedValue;
            serializer.Serialize (writer, value.ToString ());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter ();
    }
}