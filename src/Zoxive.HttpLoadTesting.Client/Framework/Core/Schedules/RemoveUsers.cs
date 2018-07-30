using System;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Framework.Core.Schedules
{
    public class RemoveUsers : ISchedule
    {
        public RemoveUsers(int usersToRemove, int usersEvery, int seconds)
        {
            Users = usersToRemove;
            UsersEvery = usersEvery;
            Seconds = seconds;
        }

        public int Seconds { get; }

        public int UsersEvery { get; }

        public int Users { get; }

        public ScheduleType Type => ScheduleType.RemoveUsers;

        private int? _initialUserCount;
        private double? _lastIteration;

        public ScheduleResult Execute(ITestExecutionContext context)
        {
            if (!_initialUserCount.HasValue)
            {
                _initialUserCount = context.CurrentUsers;
            }

            var totalSeconds = context.TotalSeconds;
            var doneUserCount = _initialUserCount.Value - Users;
            var shouldRemoveUsers = context.CurrentUsers > doneUserCount;

            var enoughTimeHasElapsed = !_lastIteration.HasValue || (totalSeconds - _lastIteration.Value) > Seconds;

            if (shouldRemoveUsers && enoughTimeHasElapsed)
            {
                Console.WriteLine("Removed {0} Users", UsersEvery);

                _lastIteration = context.TotalSeconds;

                return new ScheduleResult(false, -UsersEvery);
            }

            if(shouldRemoveUsers)
            {
                return new ScheduleResult(false);
            }

            return new ScheduleResult(true);
        }
    }
}