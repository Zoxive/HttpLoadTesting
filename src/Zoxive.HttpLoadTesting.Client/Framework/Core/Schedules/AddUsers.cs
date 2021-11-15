using System;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Framework.Core.Schedules
{
    public class AddUsers : ISchedule
    {
        public AddUsers(int totalUsers, int usersEvery, double seconds)
        {
            Users = totalUsers;
            UsersEvery = usersEvery;
            AddUsersEverySeconds = seconds;
        }

        public double AddUsersEverySeconds { get; }

        public int UsersEvery { get; }

        public int Users { get; }

        public ScheduleType Type => ScheduleType.AddUsers;

        private int? _initialUserCount;
        private double? _lastIteration;

        public ScheduleResult Execute(ITestExecutionContext context)
        {
            if (!_initialUserCount.HasValue)
            {
                _initialUserCount = context.CurrentUsers;

                Console.WriteLine($"Add Users Started. Adding {UsersEvery} every {AddUsersEverySeconds} seconds for a total of {Users}");
            }

            var totalSeconds = context.TotalSeconds;
            var doneUserCount = _initialUserCount.Value + Users;
            var needsMoreUsers = context.CurrentUsers < doneUserCount;

            var enoughTimeHasElapsed = !_lastIteration.HasValue || (totalSeconds - _lastIteration.Value) > AddUsersEverySeconds;

            if (!needsMoreUsers || !enoughTimeHasElapsed)
            {
                return new ScheduleResult(!needsMoreUsers);
            }

            _lastIteration = context.TotalSeconds;

            return new ScheduleResult(false, UsersEvery);
        }
    }
}
