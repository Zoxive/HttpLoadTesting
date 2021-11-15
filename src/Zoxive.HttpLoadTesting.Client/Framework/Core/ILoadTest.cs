using System.Threading;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Http;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public interface ILoadTest
    {
        string Name { get; }

        Task Initialize(ILoadTestHttpClient loadLoadTestHttpClient);

        Task Execute(IUserLoadTestHttpClient loadLoadTestHttpClient, CancellationToken? cancellationToken = null);
    }
}
