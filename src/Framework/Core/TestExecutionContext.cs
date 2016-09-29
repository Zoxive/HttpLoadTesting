using System;
using System.Collections.Generic;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public class TestExecutionContext : ITestExecutionContextInternal
    {
        private readonly IList<User> _currentUsers;
        private readonly int _startingTick;

        public TestExecutionContext()
        {
            _startingTick = Env.Tick;

            Step = 0;
            _currentUsers = new List<User>();
        }

        public int CurrentUsers => _currentUsers.Count;

        private double TickSinceStarting => Env.Tick - _startingTick;

        public double TotalSeconds => TickSinceStarting / 1000;

        public double TotalMinutes => TotalSeconds / 60;

        public int Step { get; private set; }

        public void NewStep()
        {
            Step++;
        }

        public void Finished()
        {
            //
        }

        public void ModifyUsers(Action<IList<User>> func)
        {
            lock (_currentUsers)
            {
                func(_currentUsers);
            }
        }
    }
}