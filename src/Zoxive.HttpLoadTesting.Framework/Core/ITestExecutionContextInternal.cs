using System;
using System.Collections.Generic;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    internal interface ITestExecutionContextInternal : ITestExecutionContext
    {
        void ModifyUsers(Action<IList<User>> func);
    }
}