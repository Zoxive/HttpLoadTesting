using System;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public class Env
    {
        public static int Tick => Environment.TickCount & int.MaxValue;
    }
}
