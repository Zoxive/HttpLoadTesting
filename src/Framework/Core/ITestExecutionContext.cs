namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public interface ITestExecutionContext
    {
        int Step { get; }

        int CurrentUsers { get; }

        double TotalSeconds { get; }

        double TotalMinutes { get; }

        void NewStep();
    }
}
