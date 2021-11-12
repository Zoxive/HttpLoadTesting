using System;
using System.Collections.Generic;

namespace Zoxive.HttpLoadTesting.Framework.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> OrEmptyArray<T>(this IEnumerable<T>? enumerable)
        {
            return enumerable ?? ArraySegment<T>.Empty;
        }
    }
}