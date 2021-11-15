namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public interface ITestExecutionContextInternal : ITestExecutionContext
    {
        int UserInitializing();

        void UserInitialized(User newUser);

        void RemoveUsers(int count);
    }
}