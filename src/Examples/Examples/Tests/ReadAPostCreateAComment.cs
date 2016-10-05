using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Http.Json;

namespace Zoxive.HttpLoadTesting.Examples.Examples.Tests
{
    public class ReadAPostCreateAComment : ILoadTest
    {
        public string Name => "ReadAPostCreateAComment";

        private const string PostId = "PostId";

        // Example of initialing TestState by using the client itself
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

            await loadTestHttpClient.DelayUserThink();

            var comment = new Dictionary<string, object>
            {
                {"name", "HttpLoadTesting"},
                {"email", "vel+minus+molestias+voluptatum@omnis.com"},
                {"body", "Comment body"}
            };

            await loadTestHttpClient.Post($"posts/{postId}/comments", comment);
        }
    }
}
