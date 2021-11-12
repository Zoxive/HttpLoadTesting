using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Zoxive.HttpLoadTesting.Framework.Http.Json
{
    public static class HttpResponseMessageJsonExtensions
    {
        public static Task<T?> AsJsonAsync<T>(this HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
                throw new Exception($"Failed Converting HTTP Response to JSON. StatusCode was Not Success. {responseMessage.StatusCode}");

            return responseMessage.Content.ReadFromJsonAsync<T>();
        }

        public static Task<JsonNode?> AsJsonAsync(this HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
                throw new Exception($"Failed Converting HTTP Response to JSON. StatusCode was Not Success. {responseMessage.StatusCode}");

            return responseMessage.Content.ReadFromJsonAsync<JsonNode>();
        }
    }
}