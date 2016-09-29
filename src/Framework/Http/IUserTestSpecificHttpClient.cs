using System;
using System.Collections.Generic;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Framework.Http
{
    public interface IUserTestSpecificHttpClient : ILoadTestHttpClient, IDisposable
    {
        IReadOnlyList<HttpStatusResult> StatusResults();

        long UserDelay();
    }
}