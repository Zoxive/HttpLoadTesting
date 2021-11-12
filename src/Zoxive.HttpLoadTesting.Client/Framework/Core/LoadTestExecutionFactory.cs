using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Framework.Core
{
    public class LoadTestExecutionFactory
    {
        private readonly ClientOptions _options;

        public LoadTestExecutionFactory(ClientOptions options)
        {
            _options = options;
        }

        public ILoadTestExecution Create(IReadOnlyList<IHttpUser> users)
        {
            return new LoadTestExecution(users, _options);
        }
    }
}
