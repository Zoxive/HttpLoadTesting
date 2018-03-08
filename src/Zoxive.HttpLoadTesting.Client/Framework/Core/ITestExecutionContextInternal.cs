namespace Zoxive.HttpLoadTesting.Framework.Core
{
    internal interface ITestExecutionContextInternal : ITestExecutionContext
    {
        int UserInitializing();

        void UserInitialized(User newUser);

        void RemoveUsers(int count);
    }
}