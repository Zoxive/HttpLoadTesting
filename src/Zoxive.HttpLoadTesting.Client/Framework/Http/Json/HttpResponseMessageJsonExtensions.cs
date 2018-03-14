using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Zoxive.HttpLoadTesting.Framework.Http.Json
{
    public static class HttpResponseMessageJsonExtensions
    {
        public static JToken AsJson(this Task<HttpResponseMessage> taskResponseMessage)
        {
            return taskResponseMessage.Result.AsJson();
        }

        public static JToken AsJson(this HttpResponseMessage responseMessage)
        {
            var intStatusCode = (int)responseMessage.StatusCode;
            if (intStatusCode >= 400)
            {
                throw new Exception($"Failed Converting HTTP Response to JSON. StatusCode was Not Success. {intStatusCode}");
            }

            using (var sr = new StreamReader(responseMessage.Content.ReadAsStreamAsync().Result))
            using (var reader = new JsonTextReader(sr))
            {
                var serializer = new JsonSerializer();

                var o = serializer.Deserialize(reader);

                if (!(o is JToken result))
                {
                    throw new Exception("Failed Converting HTTP Response to JSON");
                }

                return result;
            }
        }
    }
}