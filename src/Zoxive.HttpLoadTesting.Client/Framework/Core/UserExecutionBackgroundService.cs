using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public sealed class UserExecutionBackgroundService : BackgroundService
    {
        private readonly UserExecutingQueue _userExecutingQueue;
        private readonly TestExecutionToken _testExecutionToken;

        private readonly ConcurrentDictionary<User, Task> _users;

        public UserExecutionBackgroundService(UserExecutingQueue userExecutingQueue, TestExecutionToken testExecutionToken)
        {
            _userExecutingQueue = userExecutingQueue;
            _testExecutionToken = testExecutionToken;
            _users = new ConcurrentDictionary<User, Task>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var testToken = _testExecutionToken.Token;

            testToken.Register(() =>
            {
                foreach (var (user, _) in _users)
                {
                    user.Stop();
                    _users.TryRemove(user, out _);
                }
            });

            await foreach (var user in _userExecutingQueue.ReadAllAsync(stoppingToken))
            {
                StartRunningUser(user, testToken);
            }
        }

        private void StartRunningUser(User user, CancellationToken cancellationToken)
        {
            _users.TryAdd(user, Task.Run(async () =>
            {
                if (!user.Initialized)
                    await user.Initialize();

                // User Failed dont loop and run..
                if (!user.Initialized)
                    return;

                while (!cancellationToken.IsCancellationRequested && user.IsRunning)
                {
                    await user.Run();
                }
            }, cancellationToken));

            user.OnStop += User_OnStop;
        }

        private void User_OnStop(object? sender, EventArgs e)
        {
            if (sender is User user)
            {
                _users.TryRemove(user, out _);
            }
        }
    }

    public sealed class UserExecutingQueue
    {
        private readonly Channel<User> _channel = Channel.CreateUnbounded<User>(new UnboundedChannelOptions{ SingleWriter = true, SingleReader = true });

        public bool TryWrite(User user)
        {
            return _channel.Writer.TryWrite(user);
        }

        public IAsyncEnumerable<User> ReadAllAsync(CancellationToken cancellationToken)
        {
            return _channel.Reader.ReadAllAsync(cancellationToken);
        }

        /*
        public void BeginDrain()
        {
            if (!_channel.Writer.TryComplete())
            {
                //_logger.LogError(exception, "Failed to mark the channel as completed");
            }
        }
        */
    }
}