using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Extensions;
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

        public async Task Execute(IUserLoadTestHttpClient loadLoadTestHttpClient)
        {
            //await loadLoadTestHttpClient.DelayUserClick();

            await loadLoadTestHttpClient.Get("");
        }
    }
}
