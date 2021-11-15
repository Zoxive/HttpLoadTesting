using System;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public static class Env
    {
        public static int Milliseconds => Environment.TickCount & int.MaxValue;
    }
}
