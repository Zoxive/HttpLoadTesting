using System.Collections.Generic;

namespace Zoxive.HttpLoadTesting.Client.Domain
{
    public static class ListExtensions
    {
        public static IEnumerable<IReadOnlyList<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            var nextbatch = new List<T>(batchSize);
            foreach (var item in source)
            {
                nextbatch.Add(item);

                if (nextbatch.Count != batchSize) continue;

                yield return nextbatch;
                nextbatch.Clear();
            }

            if (nextbatch.Count > 0)
                yield return nextbatch;
        }
    }
}
