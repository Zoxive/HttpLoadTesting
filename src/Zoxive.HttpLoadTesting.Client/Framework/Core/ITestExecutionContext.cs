namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public interface ITestExecutionContext
    {
        int CurrentUsers { get; }

        double TotalSeconds { get; }

        double TotalMinutes { get; }
    }
}
