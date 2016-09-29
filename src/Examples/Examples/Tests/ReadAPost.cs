using System;
using System.Linq;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Http.Json;

namespace Zoxive.HttpLoadTesting.Examples.Examples.Tests
{
    public class ReadAPost : ILoadTest
    {
        public string Name => "ReadAPost";

        private const string PostId = "PostId";

        public async Task Initialize(ILoadTestHttpClient loadTestHttpClient)
        {
            var posts = (await loadTestHttpClient.Get("posts?_start=0&_limit=1")).AsJson();

            var post = posts.FirstOrDefault();
            if (post == null)
            {
                throw new Exception("Failing finding a post");
            }

            loadTestHttpClient.TestState.Add(PostId, post.Value<string>("id"));
        }

        public async Task Execute(ILoadTestHttpClient loadTestHttpClient)
        {
            await loadTestHttpClient.DelayUserClick();

            var postId = (string)loadTestHttpClient.TestState[PostId];

            await loadTestHttpClient.Get($"posts/{postId}");
        }
    }
}
