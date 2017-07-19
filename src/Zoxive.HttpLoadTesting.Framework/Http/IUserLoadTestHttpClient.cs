using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Framework.Http
{
    public interface IUserLoadTestHttpClient : ILoadTestHttpClient
    {
        long UserDelay { get; }

        IReadOnlyList<HttpStatusResult> StatusResults();

        Task LogUserDelay(Func<Task> func);
    }
}