﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Extensions;
using Zoxive.HttpLoadTesting.Framework.Http;
using Zoxive.HttpLoadTesting.Framework.Http.Json;

namespace Examples.Tests
{
    public class ReadAPostCreateAComment : ILoadTest
    {
        public string Name => "ReadAPostCreateAComment";

        private const string PostId = "PostId";

        // Example of initialing TestState by using the client itself
        public async Task Initialize(ILoadTestHttpClient loadLoadTestHttpClient)
        {
            var posts = await (await loadLoadTestHttpClient.Get("posts?_start=0&_limit=1")).AsJsonAsync<IReadOnlyList<PostListItem>>();

            var post = posts?.FirstOrDefault();
            if (post == null)
            {
                throw new Exception("Failing finding a post");
            }

            loadLoadTestHttpClient.TestState.Add(PostId, post.Id);
        }

        public async Task Execute(IUserLoadTestHttpClient loadLoadTestHttpClient, CancellationToken? cancellationToken = null)
        {
            await loadLoadTestHttpClient.DelayUserClick();

            var postId = (string)loadLoadTestHttpClient.TestState[PostId];

            await loadLoadTestHttpClient.Get($"posts/{postId}");

            await loadLoadTestHttpClient.DelayUserThink();

            var comment = new Dictionary<string, object>
            {
                {"name", "HttpLoadTesting"},
                {"email", "vel+minus+molestias+voluptatum@omnis.com"},
                {"body", "Comment body"}
            };

            await loadLoadTestHttpClient.Post($"posts/{postId}/comments", comment);
        }
    }

    public sealed class PostListItem
    {
        public int Id { get; set; }
    }
}
