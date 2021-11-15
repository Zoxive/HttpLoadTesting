using System.Collections.Generic;
using System.Threading;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public class TestExecutionContext : ITestExecutionContextInternal
    {
        private readonly IList<User> _currentUsers;
        private readonly int _startingTick;
        private int _usersInitializing;

        public TestExecutionContext()
        {
            _startingTick = Env.Milliseconds;

            _currentUsers = new List<User>();
            _usersInitializing = 0;
        }

        private double TickSinceStarting => Env.Milliseconds - _startingTick;

        public double TotalSeconds => TickSinceStarting / 1000;

        public double TotalMinutes => TotalSeconds / 60;

        public int CurrentUsers
        {
            get
            {
                lock (_currentUsers)
                {
                    return _currentUsers.Count;
                }
            }
        }

        public void UserInitialized(User newUser)
        {
            lock (_currentUsers)
            {
                _currentUsers.Add(newUser);
                Interlocked.Decrement(ref _usersInitializing);
            }
        }

        public void RemoveUsers(int count)
        {
            lock (_currentUsers)
            {
                for (var i = 0; i < count; i++)
                {
                    var user = _currentUsers[^1];
                    user.Stop();
                    _currentUsers.Remove(user);
                }
            }
        }

        public int UserInitializing()
        {
            lock (_currentUsers)
            {
                var initializing = Interlocked.Increment(ref _usersInitializing);
                return _currentUsers.Count + initializing;
            }
        }
    }
}