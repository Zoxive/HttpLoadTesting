using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Framework.Core
{
    public delegate void UserIterationFinished(UserIterationResult result);

    // TODO list
    /// What "Schedule" is currently running (Time started, time left?)
    /// Current User Count
    /// Save output from cli that can later open in web client

    public class LoadTestExecution : ILoadTestExecution
    {
        private readonly IReadOnlyList<IHttpUser> _httpUsers;
        private readonly IReadOnlyList<ILoadTest> _loadTests;

        public event UserIterationFinished UserIterationFinished;

        public LoadTestExecution(IReadOnlyList<IHttpUser> httpUsers, IReadOnlyList<ILoadTest> loadTests)
        {
            _httpUsers = httpUsers;
            _loadTests = loadTests;
        }

        public async Task Execute(IReadOnlyList<ISchedule> schedule, CancellationToken? token = null)
        {
            Console.WriteLine("Loaded {0} Tests.", _loadTests.Count);

            var context = new TestExecutionContext();

            var done = false;

            var scheduleIdx = 0;

            while (!done)
            {
                var startTick = Env.Tick;

                if (Canceled(token))
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

                double tickSinceStart = Env.Tick - startTick;

                if (!done && tickSinceStart < 1000)
                {
                    await Task.Delay((int) (1000 - tickSinceStart));
                }
            }
        }

        private static void Shutdown(TestExecutionContext context)
        {
            Console.WriteLine("Stopping Users");
            RemoveUsers(context.CurrentUsers, context);
            Console.WriteLine("Users Stopped");

            context.Finished();

            Console.WriteLine();

            Console.WriteLine("Total Test Time: {0} Minutes", context.TotalMinutes);
        }

        private static bool Canceled(CancellationToken? token)
        {
            if (token?.IsCancellationRequested == true)
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

        private async Task AddNewUsers(int usersChanged, ITestExecutionContextInternal context)
        {
            Console.WriteLine("Adding {0} Users", usersChanged);

            for (var i = 0; i < usersChanged; i++)
            {
                var userNum = context.CurrentUsers + 1;

                var httpUser = GetNextHttpUser(userNum);

                await Task.Run(async () =>
                {
                    var user = new User(userNum, _loadTests, httpUser);

                    Console.WriteLine($"Initializing User {userNum}");

                    try
                    {
                        await user.Initialize();
                        context.ModifyUsers(list => list.Add(user));

                        Console.WriteLine($"Added User {userNum}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed initializing User {userNum}");
                        Console.Write(ex);
                        return;
                    }

                    await user.Run(result => UserIterationFinished?.Invoke(result));
                });
            }
        }

        private IHttpUser GetNextHttpUser(int userNum)
        {
            var httpClientIdx = userNum%_httpUsers.Count;
            return _httpUsers[httpClientIdx];
        }

        private static void RemoveUsers(int usersChanged, ITestExecutionContextInternal context)
        {
            Console.WriteLine("Removing {0} Users", usersChanged);

            for (var i = 0; i < usersChanged; i++)
            {
                context.ModifyUsers(list =>
                {
                    var user = list[list.Count - 1];
                    user.Stop();

                    list.Remove(user);
                });
            }
        }
    }
}
