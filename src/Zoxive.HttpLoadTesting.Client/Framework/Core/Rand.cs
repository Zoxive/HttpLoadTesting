using System;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    internal static class Rand
    {
        private static readonly Random R = new Random();

        internal static int Random(int min, int max)
        {
            return R.Next(min, max);
        }
    }
}