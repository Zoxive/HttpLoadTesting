using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public interface ISchedule
    {
        ScheduleType Type { get; }

        ScheduleResult Execute(ITestExecutionContext context);
    }
}