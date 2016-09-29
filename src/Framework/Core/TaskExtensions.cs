using System;
using System.Threading.Tasks;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public static class TaskExtensions
    {
        public static async Task Retry(Func<Task> func, int retryCount = 3)
        {
            for (var i = 0; i < retryCount; i++)
            {
                try
                {
                    await func();
                }
                catch (Exception)
                {
                    if (i >= retryCount)
                    {
                        throw;
                    }
                    // EAT IT FOR BREAKFEST
                }
            }
        }
    }
}
