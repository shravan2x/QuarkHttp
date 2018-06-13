using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace QuarkHttp.Payloads.Json
{
    // TODO: Find out how to dispose of things correctly.
    /*public class QJsonPayload : QPayload
    {
        private static readonly JsonSerializer DefaultSerializer = JsonSerializer.CreateDefault();

        public QJsonPayload(object obj, JsonSerializer serializer = null, Encoding encoding = null) : base(ToStream(obj, serializer, encoding)) { }

        private static Stream ToStream(object obj, JsonSerializer serializer = null, Encoding encoding = null)
        {
            if (serializer == null)
                serializer = DefaultSerializer;
            if (encoding == null)
                encoding = Encoding.UTF8;

            MemoryStream outputStream = new MemoryStream();
            serializer.Serialize(new StreamWriter(outputStream, encoding), obj);

            return outputStream;
        }
    }*/
}
