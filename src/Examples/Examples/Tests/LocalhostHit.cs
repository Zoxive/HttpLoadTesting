using System.Collections.Generic;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Http;

namespace Examples.Tests
{
    public class LocalhostHit : ILoadTest
    {
        public string Name => nameof(LocalhostHit);

        public Task Initialize(ILoadTestHttpClient loadLoadTestHttpClient)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IUserLoadTestHttpClient loadLoadTestHttpClient)
        {
            const int num = 100;
            var requests = new List<Task>();

            for (var i = 0; i < num; i++)
            {
                requests.Add(loadLoadTestHttpClient.Get(""));
            }

            return Task.WhenAll(requests);
        }
    }
}
