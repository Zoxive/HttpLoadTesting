namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public interface ITestExecutionContextInternal : ITestExecutionContext
    {
        int UserInitializing();

        void AddNewUser(User newUser);

        void RemoveUsers(int count);
    }
}