using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Zoxive.HttpLoadTesting.Client.Domain.Database.Migrations;
using Zoxive.HttpLoadTesting.Framework.Core;
using Zoxive.HttpLoadTesting.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client
{
    public class WebClient
    {
        public static void Run(IReadOnlyList<IHttpUser> users, IReadOnlyList<ISchedule> schedule, IHttpStatusResultService httpStatusResultService, ClientOptions clientOptions)
        {
            Console.CancelKeyPress += ((sender, cancelEventArgs) =>
            {
                clientOptions.CancelTokenSource.Cancel();
                cancelEventArgs.Cancel = true;
            });

            var webUi = Program.StartAsync(users, schedule, httpStatusResultService, clientOptions);

            Task.WaitAll(webUi);
        }

        public static void Run(IReadOnlyList<IHttpUser> users, IReadOnlyList<ISchedule> schedule, IHttpStatusResultService httpStatusResultService, string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            Patch1.Frequency = config.GetValue<long?>("frequency");

            var clientOptions = new ClientOptions(config.GetValue<string>("databaseFile"), new CancellationTokenSource());

            Run(users, schedule, httpStatusResultService, clientOptions);
        }
    }
}