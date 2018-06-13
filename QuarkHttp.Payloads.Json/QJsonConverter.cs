using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace QuarkHttp.Payloads.Json
{
    public static class QJsonConverter
    {
        private static readonly JsonSerializer DefaultSerializer = JsonSerializer.CreateDefault();

        // TODO: Should be made async after https://github.com/JamesNK/Newtonsoft.Json/issues/66.
        public static T AsJson<T>(this QPayload payload, JsonSerializer serializer = null)
        {
            if (serializer == null)
                serializer = DefaultSerializer;

            using (StreamReader payloadReader = new StreamReader(payload.Stream))
            using (JsonTextReader payloadJsonReader = new JsonTextReader(payloadReader))
                return serializer.Deserialize<T>(payloadJsonReader);
        }
    }
}
