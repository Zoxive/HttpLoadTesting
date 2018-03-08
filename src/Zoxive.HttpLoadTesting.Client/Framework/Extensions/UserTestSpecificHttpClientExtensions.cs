using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Http;

namespace Zoxive.HttpLoadTesting.Framework.Extensions
{
    public static class UserTestSpecificHttpClientExtensions
    {
        public static Task DelayUserClick(this IUserLoadTestHttpClient client, int min = 500, int max = 1000)
        {
            return client.LogUserDelay(() => Task.Delay(Rand.Random(min, max)));
        }

        public static Task DelayUserThink(this IUserLoadTestHttpClient client, int min = 500, int max = 3000)
        {
            return client.LogUserDelay(() => Task.Delay(Rand.Random(min, max)));
        }
    }
}
