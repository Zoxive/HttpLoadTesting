using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Framework.Core
{
    public class LoadTestExecutionFactory
    {
        private readonly ClientOptions _options;
        private readonly HostRef _host;

        public LoadTestExecutionFactory(ClientOptions options, HostRef host)
        {
            _options = options;
            _host = host;
        }

        public ILoadTestExecution Create(IReadOnlyList<IHttpUser> users)
        {
            return new LoadTestExecution(users, _options, _host);
        }
    }
}
