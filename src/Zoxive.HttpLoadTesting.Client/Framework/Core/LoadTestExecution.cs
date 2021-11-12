using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Client;
using Zoxive.HttpLoadTesting.Client.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    // TODO move events to a class/interface to imeplement
    public delegate void UserIterationFinished(UserIterationResult result);

    public class LoadTestExecution : ILoadTestExecution
    {
        private readonly IReadOnlyList<IHttpUser> _httpUsers;
        private readonly ClientOptions _options;
        private readonly HostRef _host;
        private Stopwatch _executionTimestamp;

        public event UserIterationFinished? UserIterationFinished;

        public LoadTestExecution(IReadOnlyList<IHttpUser> httpUsers, ClientOptions options, HostRef host)
        {
            _httpUsers = httpUsers;
            _options = options;
            _host = host;
            _executionTimestamp = new Stopwatch();
        }

        public async Task Execute(IReadOnlyList<ISchedule> schedule) 
        {
            Console.WriteLine($"Loaded {_httpUsers.SelectMany(u => u.Tests).Distinct().Count()} Tests.");

            var context = new TestExecutionContext();

            var done = false;

            var scheduleIdx = 0;

            _executionTimestamp.Restart();

            while (!done)
            {
                var startTick = Env.Milliseconds;

                if (Canceled())
                {
                    Shutdown(context);
                    break;
                }

                var scheduleItem = schedule[scheduleIdx];

                if (await Execute(scheduleItem, context))
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
                    await Task.Delay((int) (1000 - tickSinceStart));
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
                _host.StopApplication();
            }
        }

        private bool Canceled()
        {
            if (_options.CancelTokenSource.Token.IsCancellationRequested == true)
            {
                Console.WriteLine("Canceling..");

                return true;
            }

            return false;
        }

        private async Task<bool> Execute(ISchedule scheduleItem, ITestExecutionContext context)
        {
            var result = scheduleItem.Execute(context);

            if (result.UsersChanged != 0)
            {
                var internalContext = (ITestExecutionContextInternal)context;
                if (result.UsersChanged > 0)
                {
                    await AddNewUsers(result.UsersChanged, internalContext);
                }
                else
                {
                    RemoveUsers(result.UsersChanged * -1, internalContext);
                }
            }

            return result.ScheduleComplete;
        }

        private Task AddNewUsers(int usersChanged, ITestExecutionContextInternal context)
        {
            Console.WriteLine("Adding {0} Users", usersChanged);

            var addUserTasks = new List<Task>();

            for (var i = 0; i < usersChanged; i++)
            {
                var addUserTask = Task.Run(async () =>
                {
                    var userNum = context.UserInitializing();

                    var httpUser = GetNextHttpUser(userNum);

                    // TODO
#pragma warning disable IDISP001
                    var user = new User(userNum, httpUser);
#pragma warning restore IDISP001

                    Console.WriteLine($"Initializing User {userNum}");

                    try
                    {
                        await user.Initialize();
                        context.UserInitialized(user);

                        Console.WriteLine($"Added User {userNum}");
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"GAVE UP Trying to initialize User {userNum}");
                        return;
                    }

                    await user.Run(() => _executionTimestamp.Elapsed, result => UserIterationFinished?.Invoke(result));
                });

                addUserTasks.Add(addUserTask);
            }

            return Task.WhenAll(addUserTasks);
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
