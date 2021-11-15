using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Framework.Core
{
    public class LoadTestExecutionFactory
    {
        private readonly ClientOptions _options;
        private readonly UserExecutingQueue _userExecutingQueue;

        public LoadTestExecutionFactory(ClientOptions options, UserExecutingQueue userExecutingQueue)
        {
            _options = options;
            _userExecutingQueue = userExecutingQueue;
        }

        public ILoadTestExecution Create(IReadOnlyList<IHttpUser> users)
        {
            return new LoadTestExecution(users, _options, _userExecutingQueue);
        }
    }
}
