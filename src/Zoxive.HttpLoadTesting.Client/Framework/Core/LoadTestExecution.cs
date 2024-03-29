﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    // TODO move events to a class/interface to imeplement
    public delegate void UserIterationFinished(UserIterationResult result);

    public class LoadTestExecution : ILoadTestExecution
    {
        private readonly IReadOnlyList<IHttpUser> _httpUsers;
        private readonly ClientOptions _options;
        private readonly UserExecutingQueue _executingQueue;
        private readonly Stopwatch _executionTimestamp;

        #pragma warning disable CS0067
        public event UserIterationFinished? UserIterationFinished;
        #pragma warning restore CS0067

        public LoadTestExecution(IReadOnlyList<IHttpUser> httpUsers, ClientOptions options, UserExecutingQueue executingQueue)
        {
            _httpUsers = httpUsers;
            _options = options;
            _executingQueue = executingQueue;
            _executionTimestamp = new Stopwatch();
        }

        public async Task Execute(IReadOnlyList<ISchedule> schedule, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Loaded {_httpUsers.SelectMany(u => u.Tests).Distinct().Count()} Tests.");

            var context = new TestExecutionContext();

            var done = false;

            var scheduleIdx = 0;

            _executionTimestamp.Restart();

            while (!done)
            {
                var startTick = Env.Milliseconds;

                if (Canceled(cancellationToken))
                {
                    Shutdown(context);
                    break;
                }

                var scheduleItem = schedule[scheduleIdx];

                if (await Execute(scheduleItem, context, cancellationToken))
                {
                    scheduleIdx++;

                    var wasLastScheduleItem = scheduleIdx == schedule.Count;
                    if (wasLastScheduleItem)
                    {
                        done = true;

                        Shutdown(context);
                    }
                }

                double tickSinceStart = Env.Milliseconds - startTick;

                if (!done && tickSinceStart < 1000)
                {
                    await Task.Delay((int) (1000 - tickSinceStart), cancellationToken);
                }
            }
        }

        private void Shutdown(ITestExecutionContextInternal context)
        {
            Console.WriteLine("Stopping Users");
            RemoveUsers(context.CurrentUsers, context);
            Console.WriteLine("Users Stopped");

            Console.WriteLine();

            Console.WriteLine("Total Test Time: {0} Minutes", context.TotalMinutes);

            if (_options.StopApplicationWhenComplete)
            {
                throw new NotImplementedException("TODO implement stop when complete");
            }
        }

        private static bool Canceled(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Canceling..");

                return true;
            }

            return false;
        }

        private async Task<bool> Execute(ISchedule scheduleItem, ITestExecutionContext context, CancellationToken cancellationToken)
        {
            var result = scheduleItem.Execute(context);

            if (result.UsersChanged != 0)
            {
                var internalContext = (ITestExecutionContextInternal)context;
                if (result.UsersChanged > 0)
                {
                    await AddNewUsers(result.UsersChanged, internalContext, cancellationToken);

                    Console.WriteLine("done adding users");
                }
                else
                {
                    RemoveUsers(result.UsersChanged * -1, internalContext);
                }
            }

            return result.ScheduleComplete;
        }

        private Task AddNewUsers(int usersChanged, ITestExecutionContextInternal context, CancellationToken cancellationToken)
        {
            Console.WriteLine("Adding {0} Users", usersChanged);

            var addUserTasks = new List<Task>();

            for (var i = 0; i < usersChanged; i++)
            {
                var addUserTask = Task.Run(async () =>
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    var userNum = context.UserInitializing();

                    var httpUser = GetNextHttpUser(userNum);

#pragma warning disable IDISP001
                    var user = new User(userNum, httpUser, () => _executionTimestamp.Elapsed, result => UserIterationFinished?.Invoke(result));
#pragma warning restore IDISP001

                    Console.WriteLine($"Initializing User {userNum}");

                    context.AddNewUser(user);

                    if (_options.InitializeBeforeAddingUsers)
                    {
                        await user.Initialize();
                    }

                    if (!_executingQueue.TryWrite(user))
                    {
                        throw new Exception("Failed to add user to executing queue");
                    }

                    Console.WriteLine($"Added User {userNum}");
                }, cancellationToken);

                addUserTasks.Add(addUserTask);
            }

            var cancellationTask = new TaskCompletionSource();
            cancellationToken.Register(() =>
            {
                cancellationTask.TrySetCanceled();
            });

            return Task.WhenAny(Task.WhenAll(addUserTasks), cancellationTask.Task);
        }

        private IHttpUser GetNextHttpUser(int userNum)
        {
            var httpClientIdx = userNum%_httpUsers.Count;
            return _httpUsers[httpClientIdx];
        }

        private static void RemoveUsers(int usersChanged, ITestExecutionContextInternal context)
        {
            Console.WriteLine("Removing {0} Users", usersChanged);

            context.RemoveUsers(usersChanged);
        }
    }
}
