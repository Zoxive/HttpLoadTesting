using System.Threading.Tasks;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public interface ILoadTest
    {
        string Name { get; }

        Task Initialize(ILoadTestHttpClient loadTestHttpClient);

        Task Execute(ILoadTestHttpClient loadTestHttpClient);
    }
}
