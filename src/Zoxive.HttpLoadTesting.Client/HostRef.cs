using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client
{
    public class HostRef
    {
        private readonly ClientOptions _options;

        public HostRef(ClientOptions options)
        {
            _options = options;
        }

        public void StopApplication()
        {
            _options.CancelTokenSource.Cancel();
        }
    }
}